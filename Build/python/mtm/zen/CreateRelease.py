
import sys
import os
import re

import argparse

from mtm.log.LogStreamFile import LogStreamFile
from mtm.log.LogStreamConsole import LogStreamConsole

from mtm.util.ZipHelper import ZipHelper

from mtm.util.ScriptRunner import ScriptRunner

from mtm.util.ProcessRunner import ProcessRunner
from mtm.util.SystemHelper import SystemHelper
from mtm.util.VarManager import VarManager
from mtm.config.Config import Config
from mtm.log.Logger import Logger
from mtm.util.VisualStudioHelper import VisualStudioHelper
from mtm.util.UnityHelper import UnityHelper

from mtm.util.Assert import *

import mtm.ioc.Container as Container
from mtm.ioc.Inject import Inject

ScriptDir = os.path.dirname(os.path.realpath(__file__))
RootDir = os.path.realpath(os.path.join(ScriptDir, '../../../..'))
BuildDir = os.path.realpath(os.path.join(RootDir, 'Build'))

class Runner:
    _scriptRunner = Inject('ScriptRunner')
    _log = Inject('Logger')
    _sys = Inject('SystemHelper')
    _varMgr = Inject('VarManager')
    _zipHelper = Inject('ZipHelper')
    _vsSolutionHelper = Inject('VisualStudioHelper')
    _unityHelper = Inject('UnityHelper')

    def run(self, args):
        self._args = args
        success = self._scriptRunner.runWrapper(self._runInternal)

        if not success:
            sys.exit(1)

    def _runInternal(self):
        self._varMgr.set('RootDir', RootDir)
        self._varMgr.set('BuildDir', BuildDir)
        self._varMgr.set('PythonDir', '[BuildDir]/python')
        self._varMgr.set('TempDir', '[BuildDir]/Temp')
        self._varMgr.set('AssetsDir', '[RootDir]/UnityProject/Assets')
        self._varMgr.set('ZenjectDir', '[AssetsDir]/Zenject')
        self._varMgr.set('DistDir', '[BuildDir]/Dist')
        self._varMgr.set('BinDir', '[RootDir]/AssemblyBuild/Bin')

        versionStr = self._sys.readFileAsText('[ZenjectDir]/Version.txt').strip()

        self._log.info("Found version {0}", versionStr)

        self._populateDistDir(versionStr)

        if self._args.addTag:
            self._sys.executeAndReturnOutput("git tag -a v{0} -m 'Version {0}'".format(versionStr))
            self._log.info("Incremented version to {0}! New tag was created as well", versionStr)
        else:
            self._log.info("Incremented version to {0}!  Dist directory contains the releases.  NOTE: No tags were created however", versionStr)

    def _populateDistDir(self, versionStr):

        self._sys.deleteDirectoryIfExists('[DistDir]')
        self._sys.createDirectory('[DistDir]')

        self._sys.deleteDirectoryIfExists('[TempDir]')
        self._sys.createDirectory('[TempDir]')

        try:
            self._createCSharpPackage(True, '[DistDir]/Zenject-WithAsteroidsDemo@v{0}.unitypackage'.format(versionStr))
            self._createCSharpPackage(False, '[DistDir]/Zenject@v{0}.unitypackage'.format(versionStr))

            self._createDllPackage('[DistDir]/Zenject-BinariesOnly@v{0}.unitypackage'.format(versionStr))

            self._createNonUnityZip('[DistDir]/Zenject-NonUnity@v{0}.zip'.format(versionStr))
        finally:
            self._sys.deleteDirectory('[TempDir]')

    def _createNonUnityZip(self, zipPath):

        self._log.heading('Creating non unity zip')

        tempDir = '[TempDir]/NonUnity'
        self._sys.createDirectory(tempDir)
        self._sys.clearDirectoryContents(tempDir)

        binDir = '[BinDir]/Not Unity Release'
        self._sys.deleteDirectoryIfExists(binDir)
        self._sys.createDirectory(binDir)
        self._vsSolutionHelper.buildVisualStudioProject('[RootDir]/AssemblyBuild/Zenject.sln', 'Not Unity Release')

        self._log.info('Copying Zenject dlls')
        self._sys.copyFile('{0}/Zenject.dll'.format(binDir), '{0}/Zenject.dll'.format(tempDir))
        self._sys.copyFile('{0}/Zenject.Commands.dll'.format(binDir), '{0}/Zenject.Commands.dll'.format(tempDir))

        self._zipHelper.createZipFile(tempDir, zipPath)

    def _createDllPackage(self, outputPath):

        self._log.heading('Creating {0}'.format(os.path.basename(outputPath)))

        self._varMgr.set('PackageTempDir', '[TempDir]/Packager')
        self._varMgr.set('ZenTempDir', '[PackageTempDir]/Assets/Zenject')

        self._sys.createDirectory('[PackageTempDir]')
        self._sys.createDirectory('[PackageTempDir]/ProjectSettings')

        try:
            self._log.info('Building zenject dlls')

            self._varMgr.set('ZenDllDir', '[BinDir]/Release')
            self._varMgr.set('ZenDllMetaDir', '[BuildDir]/BinaryMetas')

            self._sys.deleteDirectoryIfExists('[ZenDllDir]')
            self._sys.createDirectory('[ZenDllDir]')

            self._vsSolutionHelper.buildVisualStudioProject('[RootDir]/AssemblyBuild/Zenject.sln', 'Release')

            self._log.info('Copying Zenject dlls')

            self._sys.copyFile('[ZenDllDir]/Zenject.dll', '[ZenTempDir]/Zenject.dll')
            self._sys.copyFile('[ZenDllMetaDir]/Zenject.dll.meta', '[ZenTempDir]/Zenject.dll.meta')

            self._sys.copyFile('[ZenDllDir]/Zenject-editor.dll', '[ZenTempDir]/Editor/Zenject-editor.dll')
            self._sys.copyFile('[ZenDllMetaDir]/Zenject-editor.dll.meta', '[ZenTempDir]/Editor/Zenject-editor.dll.meta')

            self._sys.copyFile('[ZenDllDir]/Zenject.Commands.dll', '[ZenTempDir]/Zenject.Commands.dll')
            self._sys.copyFile('[ZenDllMetaDir]/Zenject.Commands.dll.meta', '[ZenTempDir]/Zenject.Commands.dll.meta')

            self._sys.copyFile('[ZenjectDir]/Version.txt', '[ZenTempDir]/Version.txt')
            self._sys.copyFile('[ZenjectDir]/Version.txt.meta', '[ZenTempDir]/Version.txt.meta')

            self._sys.copyFile('[ZenjectDir]/LICENSE.txt', '[ZenTempDir]/LICENSE.txt')
            self._sys.copyFile('[ZenjectDir]/LICENSE.txt.meta', '[ZenTempDir]/LICENSE.txt.meta')

            self._sys.copyDirectory('[ZenjectDir]/Documentation', '[ZenTempDir]/Documentation')
            self._sys.copyFile('[ZenjectDir]/Documentation.meta', '[ZenTempDir]/Documentation.meta')

            self._createUnityPackage('[PackageTempDir]', outputPath)
        finally:
            self._sys.deleteDirectory('[PackageTempDir]')

        self._log.heading('Creating {0}'.format(os.path.basename(outputPath)))

    def _createCSharpPackage(self, includeSample, outputPath):

        self._log.heading('Creating {0}'.format(os.path.basename(outputPath)))

        self._varMgr.set('PackageTempDir', '[TempDir]/Packager')
        self._varMgr.set('ZenTempDir', '[PackageTempDir]/Assets/Zenject')

        self._sys.createDirectory('[PackageTempDir]')
        self._sys.createDirectory('[PackageTempDir]/ProjectSettings')

        try:
            self._log.info('Copying Zenject to temporary directory')
            self._sys.copyDirectory('[ZenjectDir]', '[ZenTempDir]')

            self._log.info('Cleaning up Zenject directory')
            self._zipHelper.createZipFile('[ZenTempDir]/OptionalExtras/UnitTests', '[ZenTempDir]/OptionalExtras/UnitTests.zip')
            self._sys.deleteDirectory('[ZenTempDir]/OptionalExtras/UnitTests')
            self._sys.removeFile('[ZenTempDir]/OptionalExtras/UnitTests.meta')

            self._zipHelper.createZipFile('[ZenTempDir]/OptionalExtras/AutoMocking', '[ZenTempDir]/OptionalExtras/AutoMocking.zip')
            self._sys.deleteDirectory('[ZenTempDir]/OptionalExtras/AutoMocking')
            self._sys.removeFile('[ZenTempDir]/OptionalExtras/AutoMocking.meta')

            self._sys.removeFile('[ZenTempDir]/Source/Zenject.csproj')
            self._sys.removeFile('[ZenTempDir]/Source/Zenject.csproj.user')
            self._sys.removeFile('[ZenTempDir]/OptionalExtras/CommandsAndSignals/Zenject.Commands.csproj')
            self._sys.removeFile('[ZenTempDir]/OptionalExtras/CommandsAndSignals/Zenject.Commands.csproj.user')

            if not includeSample:
                self._sys.deleteDirectory('[ZenTempDir]/OptionalExtras/SampleGame')

            self._createUnityPackage('[PackageTempDir]', outputPath)
        finally:
            self._sys.deleteDirectory('[PackageTempDir]')

    def _createUnityPackage(self, projectPath, outputPath):
        self._sys.copyFile('[BuildDir]/UnityPackager/UnityPackageUtil.cs', '{0}/Assets/Editor/UnityPackageUtil.cs'.format(projectPath))

        self._log.info('Running unity to create unity package')

        self._unityHelper.runEditorFunction('[PackageTempDir]', 'Zenject.UnityPackageUtil.CreateUnityPackage')
        self._sys.move('{0}/Zenject.unitypackage'.format(projectPath), outputPath)

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

    Container.bind('ScriptRunner').toSingle(ScriptRunner)
    Container.bind('VarManager').toSingle(VarManager)
    Container.bind('SystemHelper').toSingle(SystemHelper)
    Container.bind('Logger').toSingle(Logger)
    Container.bind('ProcessRunner').toSingle(ProcessRunner)
    Container.bind('ZipHelper').toSingle(ZipHelper)
    Container.bind('VisualStudioHelper').toSingle(VisualStudioHelper)
    Container.bind('UnityHelper').toSingle(UnityHelper)

if __name__ == '__main__':

    if (sys.version_info < (3, 0)):
        print('Wrong version of python!  Install python 3 and try again')
        sys.exit(2)

    parser = argparse.ArgumentParser(description='Zenject Releaser')
    parser.add_argument('-t', '--addTag', action='store_true', help='')
    args = parser.parse_args(sys.argv[1:])

    installBindings()

    Runner().run(args)

