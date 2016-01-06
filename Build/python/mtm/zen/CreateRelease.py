
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

from mtm.util.Assert import *

import mtm.ioc.Container as Container
from mtm.ioc.Inject import Inject

ScriptDir = os.path.dirname(os.path.realpath(__file__))
RootDir = os.path.realpath(os.path.join(ScriptDir, '../../../..'))

class Runner:
    _log = Inject('Logger')
    _sys = Inject('SystemHelper')
    _varMgr = Inject('VarManager')
    _zipHelper = Inject('ZipHelper')

    def run(self):
        self._varMgr.add('RootDir', RootDir)
        self._varMgr.add('BuildDir', '[RootDir]/Build')
        self._varMgr.add('PythonDir', '[BuildDir]/python')
        self._varMgr.add('TempDir', '[BuildDir]/Temp')
        self._varMgr.add('AssetsDir', '[RootDir]/UnityProject/Assets')
        self._varMgr.add('ZenjectDir', '[AssetsDir]/Zenject')

        versionStr = self._sys.readFileAsText('[BuildDir]/Version.txt').strip()

        self._log.info("Found version {0}", versionStr)

        groups = re.match('^(\d+)\.(\d+)$', versionStr).groups()

        majorNumber = int(groups[0])
        minorNumber = int(groups[1])

        minorNumber += 1

        self._createReleaseZip(versionStr)

        self._sys.executeAndReturnOutput("git tag -a v{0}.{1} -m 'Version {0}.{1}'".format(majorNumber, minorNumber))
        self._sys.writeFileAsText('[BuildDir]/Version.txt', '{0}.{1}'.format(majorNumber, minorNumber))

        self._log.info("Incremented version to {0}.{1}. Now commit and then run 'git push --tags'\n\n", majorNumber, minorNumber)

    def _createReleaseZip(self, versionStr):
        self._log.heading('Creating release zip file')

        self._sys.createDirectory('[TempDir]')
        self._sys.clearDirectoryContents('[TempDir]')

        self._varMgr.add('ZenTempDir', '[TempDir]/Packager/Assets/Zenject')

        self._log.info('Copying Zenject to temporary directory')
        self._sys.copyDirectory('[ZenjectDir]', '[ZenTempDir]')

        self._log.info('Cleaning up Zenject directory')
        self._zipHelper.createZipFile('[ZenTempDir]/Extras/ZenjectUnitTests', '[ZenTempDir]/Extras/ZenjectUnitTests.zip')
        self._sys.deleteDirectory('[ZenTempDir]/Extras/ZenjectUnitTests')

        self._zipHelper.createZipFile('[ZenTempDir]/Extras/ZenjectAutoMocking', '[ZenTempDir]/Extras/ZenjectAutoMocking.zip')
        self._sys.deleteDirectory('[ZenTempDir]/Extras/ZenjectAutoMocking')

        self._sys.createDirectory('[TempDir]/Packager/ProjectSettings')

        self._sys.copyFile('[BuildDir]/UnityPackager/UnityPackageUtil.cs', '[TempDir]/Packager/Assets/Editor/UnityPackageUtil.cs')

        self._log.info('Creating unity package')
        self._sys.executeAndWait('"[UnityExePath]" -batchmode -nographics -quit -projectPath "[TempDir]/Packager" -executeMethod Zenject.UnityPackageUtil.CreateUnityPackage')

        self._varMgr.add('DistDir', '[BuildDir]/Dist')

        self._sys.createDirectory('[DistDir]')
        self._sys.clearDirectoryContents('[DistDir]')

        self._sys.copyFile('[TempDir]/Packager/Zenject.unitypackage', '[DistDir]/Zenject-{0}.unitypackage'.format(versionStr))

def installBindings():

    config = {
        'PathVars': {
            'UnityExePath': 'C:/Program Files/Unity/Editor/Unity.exe',
        }
    }
    Container.bind('Config').toSingle(Config, [config])

    Container.bind('LogStream').toSingle(LogStreamFile)
    Container.bind('LogStream').toSingle(LogStreamConsole, True, False)

    Container.bind('VarManager').toSingle(VarManager)
    Container.bind('SystemHelper').toSingle(SystemHelper)
    Container.bind('Logger').toSingle(Logger)
    Container.bind('ProcessRunner').toSingle(ProcessRunner)
    Container.bind('ZipHelper').toSingle(ZipHelper)

if __name__ == '__main__':

    if (sys.version_info < (3, 0)):
        print('Wrong version of python!  Install python 3 and try again')
        sys.exit(2)

    installBindings()

    Runner().run()

