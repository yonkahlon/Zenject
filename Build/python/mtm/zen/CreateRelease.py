
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
            self._createPackage(False, True, '[DistDir]/Zenject-{0}-WithAsteroidsDemo.unitypackage'.format(versionStr))
            self._createPackage(False, False, '[DistDir]/Zenject-{0}.unitypackage'.format(versionStr))
            self._createPackage(True, False, '[DistDir]/Zenject-{0}-BinariesOnly.unitypackage'.format(versionStr))

            self._createNonUnityZip('[DistDir]/Zenject-{0}-NonUnity.zip'.format(versionStr))
        finally:
            self._sys.deleteDirectory('[TempDir]')

    def _createNonUnityZip(self, zipPath):

        self._log.heading('Creating non unity zip')

        tempDir = '[TempDir]/NonUnity'
        self._sys.createDirectory(tempDir)
        self._sys.clearDirectoryContents(tempDir)

        binDir = '[BinDir]/Not Unity Release'
        self._sys.deleteDirectoryIfExists(binDir)
        self._sys.clearDirectory(binDir)
        self._vsSolutionHelper.buildVisualStudioProject('[RootDir]/AssemblyBuild/Zenject.sln', 'Not Unity Release')

        self._log.info('Copying Zenject dlls')
        self._sys.copyFile('{0}/Zenject.dll'.format(binDir), '{0}/Zenject.dll'.format(tempDir))
        self._sys.copyFile('{0}/Zenject.Commands.dll'.format(binDir), '{0}/Zenject.Commands.dll'.format(tempDir))

        self._zipHelper.createZipFile(tempDir, zipPath)

    def _createPackage(self, useDll, includeSample, outputPath):

        self._log.heading('Creating {0}'.format(os.path.basename(outputPath)))

        self._varMgr.set('PackageTempDir', '[TempDir]/Packager')
        self._varMgr.set('ZenTempDir', '[PackageTempDir]/Assets/Zenject')

        self._sys.createDirectory('[PackageTempDir]')
        self._sys.createDirectory('[PackageTempDir]/ProjectSettings')

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

        self._sys.copyFile('[BuildDir]/UnityPackager/UnityPackageUtil.cs', '[PackageTempDir]/Assets/Editor/UnityPackageUtil.cs')

        self._log.info('Running unity to create unity package')

        self._unityHelper.runEditorFunction('[PackageTempDir]', 'Zenject.UnityPackageUtil.CreateUnityPackage')

        self._sys.move('[PackageTempDir]/Zenject.unitypackage', outputPath)

        self._sys.deleteDirectory('[PackageTempDir]')

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

