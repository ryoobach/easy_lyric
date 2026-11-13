# Easy Lyric

Windows x64 desktop application for silent monitoring of internet radio streams, with synchronized lyrics, guitar chords, and sheet music for DJs/CJs.

## Initial development target
- Windows 10 x64 as the primary development and test environment.
- Windows 11 compatibility.
- Windows 7 SP1 x64 compatibility will be tested; it is not guaranteed until verified.
- Portable ZIP distribution (no installer).
- The application must never play the monitored stream through the PC speakers. Audio is received internally for analysis only.

## Current milestone
This repository starts with the desktop UI and automated Windows build pipeline. Radio stream reception, song identification, lyric retrieval, score/chord sources, and timing synchronization will be implemented and tested in later milestones.

## Build
Open `EasyLyric.sln` in Visual Studio with the .NET Framework 4.8 targeting pack, or run the GitHub Actions workflow. The workflow packages the build output as a ZIP artifact.
