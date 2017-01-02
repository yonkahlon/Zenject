@echo off

grip %~dp0\..\README.md --export .\UnityProject\Assets\Zenject\Documentation\README.html
grip %~dp0\..\Documentation\AutoMocking.md --export .\UnityProject\Assets\Zenject\Documentation\AutoMocking.html
grip %~dp0\..\Documentation\CheatSheet.md --export .\UnityProject\Assets\Zenject\Documentation\CheatSheet.html
grip %~dp0\..\Documentation\CommandsAndSignals.md --export .\UnityProject\Assets\Zenject\Documentation\CommandsAndSignals.html
grip %~dp0\..\Documentation\Factories.md --export .\UnityProject\Assets\Zenject\Documentation\Factories.html
grip %~dp0\..\Documentation\ReleaseNotes.md --export .\UnityProject\Assets\Zenject\Documentation\ReleaseNotes.html
grip %~dp0\..\Documentation\SubContainers.md --export .\UnityProject\Assets\Zenject\Documentation\SubContainers.html
grip %~dp0\..\Documentation\WritingAutomatedTests.md --export .\UnityProject\Assets\Zenject\Documentation\WritingAutomatedTests.html
