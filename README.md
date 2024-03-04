# CustomKit

CustomKit is a utility program designed for changing CS2 (Counter-Strike 2) music kits, allowing users to use completely custom-made, non-Valve music kits in the game.

## Features

- **Custom Music Kits:** Replace default CS2 music kits with your own selection of MP3 files for various in-game events.
- **Dynamic Audio Playback:** Utilizes NAudio.Wave library to play audio files asynchronously during specific game events.
- **Event-driven Logic:** CustomKit responds to in-game events and triggers audio playback accordingly, enhancing the gaming experience.

## Requirements

- **CS2 (Counter-Strike 2):** CustomKit is designed to work with CS2. Ensure that the game is installed on your system.
- **NAudio.Wave Library:** The utility relies on the NAudio.Wave library for audio playback. Make sure to include this library in your project.

## Installation

1. Clone the CustomKit repository to your local machine.
2. Build the solution using your preferred IDE or compiler.
3. Run the CustomKit executable.

## Tutorial for Users

1. Download a music kit from someone on the forum, create your own, or use the one provided from Brighter Lights.
2. Select it in the CMD form when prompted.
3. Boot up CS2.
4. Enjoy!

## Tutorial for Creators

1. Download sections of music that you want to use.
   - These sections should be separated like this:
     - action1.mp3, action2.mp3, round1.mp3 (round start 1), round2.mp3 (round start 2) - 12 seconds
     - mvp.mp3 - 11 seconds
     - roundloss.mp3, roundwin.mp3 - 12 to 24 seconds
     - mainmenu.mp3 - full song
     - deathcam.mp3 - 6 seconds
     - bombplanted.mp3 - 40s to 42s
2. Once done, create a folder in the same directory as CustomKit.exe.
3. Place all the music kit files in there.
4. Share your music kits with everyone!


## Configuration

CustomKit uses a straightforward console interface to guide users through the customization process. Follow the prompts to select folders and choose MP3 files for different in-game events.

## Contributing

Feel free to contribute to CustomKit by providing bug reports, feature suggestions, or even submitting pull requests. Let's make in-game music customization more accessible and enjoyable for the CS2 community.

## Known Issues

- In some cases, audio playback may not sync perfectly with specific in-game events. Adjusting audio files or seeking community support can help resolve such issues.

## Disclaimer

CustomKit is a third-party utility and is not affiliated with Valve Corporation or Counter-Strike: Source. Use it at your own risk, and always comply with the terms of service of the game.

## Acknowledgments

- Special thanks to the UnknownCheats community for supporting and sharing custom music kits.

Have a wonderful gaming experience with your personalized music kits! If you create a unique music kit, consider sharing it with the community on the [CustomKit thread]([#](https://www.unknowncheats.me/forum/counter-strike-2-releases/625990-custom-kit-external-custom-music-kits-counter-strike-2-a.html)https://www.unknowncheats.me/forum/counter-strike-2-releases/625990-custom-kit-external-custom-music-kits-counter-strike-2-a.html) at UnknownCheats.
