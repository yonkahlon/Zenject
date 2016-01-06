
import sys
import os
import re

from mtm.log.LogStreamFile import LogStreamFile
from mtm.log.LogStreamConsole import LogStreamConsole

from mtm.util.ZipHelper import ZipHelper

from mtm.util.ProcessRunner import ProcessRunner
from mtm.util.SystemHelper import SystemHelper
from mtm.util.VarManager import VarManager
from mtm.config.Config import Config
from mtm.log.Logger import Logger
from mtm.util.VisualStudioHelper import VisualStudioHelper

from mtm.util.Assert import *

import mtm.ioc.Container as Container
from mtm.ioc.Inject import Inject

ScriptDir = os.path.dirname(os.path.realpath(__file__))
RootDir = os.path.realpath(os.path.join(ScriptDir, '../../../..'))
BuildDir = os.path.realpath(os.path.join(RootDir, 'Build'))

class Runner:
    _log = Inject('Logger')
    _sys = Inject('SystemHelper')
    _varMgr = Inject('VarManager')
    _zipHelper = Inject('ZipHelper')
    _vsSolutionHelper = Inject('VisualStudioHelper')

    def run(self):
        self._varMgr.add('RootDir', RootDir)
        self._varMgr.add('BuildDir', BuildDir)
        self._varMgr.add('PythonDir', '[BuildDir]/python')
        self._varMgr.add('TempDir', '[BuildDir]/Temp')
        self._varMgr.add('AssetsDir', '[RootDir]/UnityProject/Assets')
        self._varMgr.add('ZenjectDir', '[AssetsDir]/Zenject')
        self._varMgr.add('DistDir', '[BuildDir]/Dist')
        self._varMgr.add('BinDir', '[RootDir]/AssemblyBuild/Bin')

        versionStr = self._sys.readFileAsText('[BuildDir]/Version.txt').strip()

        self._log.info("Found version {0}", versionStr)

        groups = re.match('^(\d+)\.(\d+)$', versionStr).groups()

        majorNumber = int(groups[0])
        minorNumber = int(groups[1])

        minorNumber += 1

        self._populateDistDir(versionStr)

        self._sys.executeAndReturnOutput("git tag -a v{0}.{1} -m 'Version {0}.{1}'".format(majorNumber, minorNumber))
        self._sys.writeFileAsText('[BuildDir]/Version.txt', '{0}.{1}'.format(majorNumber, minorNumber))

        self._log.info("Incremented version to {0}.{1}! \n\nNow commit, run 'git push --tags', then update the Releases with the contents of the Dist directory\n\n", majorNumber, minorNumber)

    def _populateDistDir(self, versionStr):

        self._sys.createDirectory('[DistDir]')
        self._sys.clearDirectoryContents('[DistDir]')

        self._createPackage(False, True, '[DistDir]/Zenject-{0}-WithAsteroidsDemo.unitypackage'.format(versionStr))
        self._createPackage(False, False, '[DistDir]/Zenject-{0}.unitypackage'.format(versionStr))
        self._createPackage(True, False, '[DistDir]/Zenject-{0}-BinariesOnly.unitypackage'.format(versionStr))

        self._createNonUnityZip('[DistDir]/Zenject-{0}-NonUnity.zip'.format(versionStr))

    def _createNonUnityZip(self, zipPath):

        self._log.heading('Creating non unity zip')

        tempDir = '[TempDir]/NonUnity'
        self._sys.createDirectory(tempDir)
        self._sys.clearDirectoryContents(tempDir)

        binDir = '[BinDir]/Not Unity Release'
        self._sys.createDirectory(binDir)
        self._sys.clearDirectoryContents(binDir)
        self._vsSolutionHelper.buildVisualStudioProject('[RootDir]/AssemblyBuild/Zenject.sln', 'Not Unity Release')

        self._log.info('Copying Zenject dlls')
        self._sys.copyFile('{0}/Zenject.dll'.format(binDir), '{0}/Zenject.dll'.format(tempDir))
        self._sys.copyFile('{0}/Zenject.Commands.dll'.format(binDir), '{0}/Zenject.Commands.dll'.format(tempDir))

        self._zipHelper.createZipFile(tempDir, zipPath)

    def _createPackage(self, useDll, includeSample, outputPath):

        self._log.heading('Creating {0}'.format(os.path.basename(outputPath)))

        self._sys.createDirectory('[TempDir]')
        self._sys.clearDirectoryContents('[TempDir]')

        self._varMgr.add('ZenTempDir', '[TempDir]/Packager/Assets/Zenject')

        if useDll:
            self._log.info('Building zenject dlls')
            self._sys.clearDirectoryContents('[BinDir]/Release')
            self._vsSolutionHelper.buildVisualStudioProject('[RootDir]/AssemblyBuild/Zenject.sln', 'Release')

            self._log.info('Copying Zenject dlls')
            self._sys.copyFile('[BinDir]/Release/Zenject.dll', '[ZenTempDir]/Zenject.dll')
            self._sys.copyFile('[BinDir]/Release/Zenject-editor.dll', '[ZenTempDir]/Editor/Zenject-editor.dll')
            self._sys.copyFile('[BinDir]/Release/Zenject.Commands.dll', '[ZenTempDir]/Zenject.Commands.dll')
        else:
            self._log.info('Copying Zenject to temporary directory')
            self._sys.copyDirectory('[ZenjectDir]', '[ZenTempDir]')

            self._log.info('Cleaning up Zenject directory')
            self._zipHelper.createZipFile('[ZenTempDir]/Extras/ZenjectUnitTests', '[ZenTempDir]/Extras/ZenjectUnitTests.zip')
            self._sys.deleteDirectory('[ZenTempDir]/Extras/ZenjectUnitTests')

            self._zipHelper.createZipFile('[ZenTempDir]/Extras/ZenjectAutoMocking', '[ZenTempDir]/Extras/ZenjectAutoMocking.zip')
            self._sys.deleteDirectory('[ZenTempDir]/Extras/ZenjectAutoMocking')

            if not includeSample:
                self._sys.deleteDirectory('[ZenTempDir]/Extras/SampleGame')

        self._sys.createDirectory('[TempDir]/Packager/ProjectSettings')
        self._sys.createDirectory('[TempDir]/Packager/Assets')

        self._sys.copyFile('[BuildDir]/UnityPackager/UnityPackageUtil.cs', '[TempDir]/Packager/Assets/Editor/UnityPackageUtil.cs')

        self._log.info('Running unity to create unity package')
        self._sys.executeAndWait('"[UnityExePath]" -batchmode -nographics -quit -projectPath "[TempDir]/Packager" -executeMethod Zenject.UnityPackageUtil.CreateUnityPackage')

        self._sys.move('[TempDir]/Packager/Zenject.unitypackage', outputPath)

def installBindings():

    config = {
        'PathVars': {
            'UnityExePath': 'C:/Program Files/Unity/Editor/Unity.exe',
            'LogPath': os.path.join(BuildDir, 'Log.txt'),
            'MsBuildExePath': 'C:/Windows/Microsoft.NET/Framework/v4.0.30319/msbuild.exe'
        },
        'Compilation': {
            'UseDevenv': False
        },
    }
    Container.bind('Config').toSingle(Config, [config])

    Container.bind('LogStream').toSingle(LogStreamFile)
    Container.bind('LogStream').toSingle(LogStreamConsole, True, False)

    Container.bind('VarManager').toSingle(VarManager)
    Container.bind('SystemHelper').toSingle(SystemHelper)
    Container.bind('Logger').toSingle(Logger)
    Container.bind('ProcessRunner').toSingle(ProcessRunner)
    Container.bind('ZipHelper').toSingle(ZipHelper)
    Container.bind('VisualStudioHelper').toSingle(VisualStudioHelper)

if __name__ == '__main__':

    if (sys.version_info < (3, 0)):
        print('Wrong version of python!  Install python 3 and try again')
        sys.exit(2)

    installBindings()

    Runner().run()

