using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;
namespace CustomKit
{
    class Program
    {
        [DllImport("Kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int size, IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);
        static public byte[] ReadBytes(Process proc, IntPtr addy, int bytes)
        {
            byte[] array = new byte[bytes];
            ReadProcessMemory(proc.Handle, addy, array, array.Length, IntPtr.Zero);
            return array;
        }

        static public byte[] ReadBytes(Process proc, IntPtr addy, int offset, int bytes)
        {
            byte[] array = new byte[bytes];
            ReadProcessMemory(proc.Handle, addy + offset, array, array.Length, IntPtr.Zero);
            return array;
        }
        static public IntPtr GetModuleBase(Process proc, string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName))
            {
                throw new InvalidOperationException("moduleName was either null or empty.");
            }

            if (proc == null)
            {
                throw new InvalidOperationException("process is invalid, check your init.");
            }

            try
            {
                if (moduleName.Contains(".exe") && proc.MainModule != null)
                {
                    return proc.MainModule.BaseAddress;
                }

                foreach (ProcessModule module in proc.Modules)
                {
                    if (module.ModuleName == moduleName)
                    {
                        return module.BaseAddress;
                    }
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Failed to find the specified module, search string might have miss-spellings.");
            }

            return IntPtr.Zero;
        }

        static public IntPtr ReadPointer(Process proc, IntPtr addy)
        {
            byte[] array = new byte[8];
            ReadProcessMemory(proc.Handle, addy, array, array.Length, IntPtr.Zero);
            return (IntPtr)BitConverter.ToInt64(array);
        }

        static public IntPtr ReadPointer(Process proc, IntPtr addy, int offset)
        {
            byte[] array = new byte[8];
            ReadProcessMemory(proc.Handle, addy + offset, array, array.Length, IntPtr.Zero);
            return (IntPtr)BitConverter.ToInt64(array);
        }

        public IntPtr ReadPointer(Process proc, long addy, long offset)
        {
            byte[] array = new byte[8];
            ReadProcessMemory(proc.Handle, (IntPtr)(addy + offset), array, array.Length, IntPtr.Zero);
            return (IntPtr)BitConverter.ToInt64(array);
        }

        public IntPtr ReadPointer(IntPtr addy, long offset)
        {
            return ReadPointer(addy, offset);
        }

        public IntPtr ReadPointer(IntPtr addy, int[] offsets)
        {
            IntPtr intPtr = addy;
            foreach (int offset in offsets)
            {
                intPtr = ReadPointer(intPtr, offset);
            }

            return intPtr;
        }

        public IntPtr ReadPointer(IntPtr addy, long[] offsets)
        {
            IntPtr intPtr = addy;
            foreach (long offset in offsets)
            {
                intPtr = ReadPointer(intPtr, offset);
            }

            return intPtr;
        }

        public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2)
        {
            return ReadPointer(addy, new int[2] { offset1, offset2 });
        }

        public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2, int offset3)
        {
            return ReadPointer(addy, new int[3] { offset1, offset2, offset3 });
        }



        static public bool ReadBool(Process proc, IntPtr address, int offset)
        {
            return BitConverter.ToBoolean(ReadBytes(proc, address + offset, 1));
        }


        static public int ReadInt(Process proc, IntPtr address, int offset)
        {
            return BitConverter.ToInt32(ReadBytes(proc, address + offset, 4));
        }
        static Dictionary<string, string> selectedAudio = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            string installDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string[] mp3Files = Directory.GetFiles(installDirectory, "*.mp3", SearchOption.AllDirectories);

            // List of target files to look for
            string[] targetFiles = {
            "mainmenu.mp3",
            "action1.mp3",
            "action2.mp3",
            "round1.mp3",
            "round2.mp3",
            "bombplanted.mp3",
            "deathcam.mp3",
            "mvp.mp3",
            "roundwin.mp3",
            "roundloss.mp3"
            };
            Dictionary<string, List<string>> audioDictionary = new Dictionary<string, List<string>>();

            foreach (string mp3File in mp3Files)
            {
                string fileName = Path.GetFileName(mp3File);

                for (int i = 0; i < targetFiles.Length; i++)
                {
                    if (fileName.Equals(targetFiles[i], StringComparison.OrdinalIgnoreCase))
                    {
                        string folderName = Path.GetFileName(Path.GetDirectoryName(mp3File));
                        if (!audioDictionary.ContainsKey(folderName))
                        {
                            audioDictionary[folderName] = new List<string>();
                        }
                        audioDictionary[folderName].Add(mp3File);
                        break;
                    }
                }
            }

            Console.WriteLine("Available folders:");
            int folderIndex = 1;
            foreach (var folderName in audioDictionary.Keys)
            {
                Console.WriteLine($"{folderIndex}. {folderName}");
                folderIndex++;
            }
            

            Console.Write("Select a folder by entering the corresponding number: ");
            if (int.TryParse(Console.ReadLine(), out int selectedFolderIndex) && selectedFolderIndex > 0 && selectedFolderIndex <= audioDictionary.Count)
            {
                string selectedFolder = audioDictionary.Keys.ElementAt(selectedFolderIndex - 1);
                List<string> selectedAudioPaths = audioDictionary[selectedFolder];

                // Use selectedAudioPaths in your code for playing audio

                Console.WriteLine($"Selected folder: {selectedFolder}");
                Console.WriteLine("Selected audio files:");
                foreach (var audioPath in selectedAudioPaths)
                {
                    Console.WriteLine($"{Path.GetFileName(audioPath)} [{audioPath}]");
                    string targetFileName = Path.GetFileName(audioPath);
                    string targetName = targetFiles.FirstOrDefault(file => targetFileName.Equals(file, StringComparison.OrdinalIgnoreCase));
                    char[] charsToTrim = { '.', 'm', 'p', '3' };
                    targetName = targetName.TrimEnd(charsToTrim);
                    if (!string.IsNullOrEmpty(targetName))
                    {
                        selectedAudio[targetName] = audioPath;
                    }
                }
            }
            var plr = new AudioPlayer();
            System.Threading.Thread.Sleep(3000);
            Process proc = null;
            while(proc==null)
            {
                
                Console.WriteLine("Please Start Up CS2");
                System.Threading.Thread.Sleep(5000);
                if (Process.GetProcessesByName("cs2").Length != 0)
                {
                    proc = Process.GetProcessesByName("cs2")[0];
                }
                
            }
            Thread kys = new Thread(new ParameterizedThreadStart(MainLogic));
            kys.Start(proc);
        }

        static void MainLogic(object obj)
        {
            var plr = new AudioPlayer();
            if (obj is Process proc)
            {
                IntPtr client = GetModuleBase(proc, "client.dll");
                IntPtr localPlayerpwnaddress = ReadPointer(proc, client, 0x1730118);
                IntPtr localPlayerctladdress = ReadPointer(proc, client, 0x190B308);
                int killall = 8;
                int killallt = 9;
                int defuse = 7;
                int failtoplant = 12;
                int plant = 1;
                int gamephase1 = 2;
                int gamephase2 = 4;
                int pMVPs = ReadInt(proc, localPlayerctladdress, 0x838);
                IntPtr GameRulesProxy = ReadPointer(proc, client, 0x1918A30);
                int previousTotalRounds = ReadInt(proc, GameRulesProxy, 0x7C);
                int previousGamePhase = ReadInt(proc, GameRulesProxy, 0x78);
                bool oonce = false;
                bool notstarted = true;
                bool mstarto = false;
                bool ftimeo = false;
                int pteam;
                int winteam;
                int wasalive = ReadInt(proc, localPlayerpwnaddress, 0x334);
                while (Process.GetProcessesByName("cs2").Length != 0)
                {

                    localPlayerctladdress = ReadPointer(proc, client, 0x190B308);
                    localPlayerpwnaddress = ReadPointer(proc, client, 0x1730118);
                    pteam = ReadInt(proc, localPlayerpwnaddress, 0x3cb);

                    GameRulesProxy = ReadPointer(proc, client, 0x1918A30);
                    int totalrounds = ReadInt(proc, GameRulesProxy, 0x7C);
                    bool planted = ReadBool(proc, GameRulesProxy, 0x9DD);
                    int RoundWinStatus = ReadInt(proc, GameRulesProxy, 0x9E0);
                    int RoundTimes = ReadInt(proc, GameRulesProxy, 0x64);
                    int RoundEndReason = ReadInt(proc, GameRulesProxy, 0xEBC);
                    int iMVPs = ReadInt(proc, localPlayerctladdress, 0x838);
                    int GamePhase = ReadInt(proc, GameRulesProxy, 0x78);
                    int isalive = ReadInt(proc, localPlayerpwnaddress, 0x334);
                    bool freezeTime = ReadBool(proc, GameRulesProxy, 0x30);
                    bool match_started = ReadBool(proc, GameRulesProxy, 0xA4);
                    if (RoundEndReason != -1)
                    {
                        if (match_started && !mstarto)
                        {
                            if (plr.isplaying)
                            {
                                plr.StopAudioAsync();
                            }
                            plr.PlayAudioAsync(selectedAudio["action1"]);
                            mstarto = true;
                        }
                        if (!freezeTime && !ftimeo && match_started)
                        {
                            if (plr.isplaying)
                            {
                                plr.StopAudioAsync();
                            }
                            if (GamePhase == gamephase1)
                            {
                                plr.PlayAudioAsync(selectedAudio["round1"]);
                            }
                            else
                            {
                                plr.PlayAudioAsync(selectedAudio["round2"]);
                            }
                            ftimeo = true;
                        }
                        if ((isalive == 0 || isalive > 100) && (wasalive > 0 && wasalive < 101))
                        {
                            if (plr.isplaying)
                            {
                                plr.StopAudioAsync();
                            }
                            wasalive = isalive;
                            plr.PlayAudioAsync(selectedAudio["deathcam"]);
                        }

                        if (isalive > 0 && isalive < 101)
                        {
                            wasalive = isalive;
                        }

                        if (GamePhase != previousGamePhase && (GamePhase == gamephase1 || GamePhase == gamephase2) && !plr.isplaying && previousGamePhase == gamephase1 || previousGamePhase == gamephase2)
                        {
                            plr.PlayAudioAsync(selectedAudio["action2"]);
                        }

                        if (totalrounds != previousTotalRounds)
                        {
                            winteam = ReadInt(proc, GameRulesProxy, 0xEB8);
                            pteam = ReadInt(proc, localPlayerctladdress, 0x3CB);
                            if (pteam != winteam && iMVPs == pMVPs)
                            {
                                plr.PlayAudioAsync(selectedAudio["roundloss"]);
                            } else if (pteam == winteam && iMVPs == pMVPs)
                            {
                                plr.PlayAudioAsync(selectedAudio["roundwin"]);
                            }
                            if (plr.isplaying)
                            {
                                plr.StopAudioAsync();
                            }
                            plr.StopAudioAsync();
                            previousTotalRounds = totalrounds;
                            if (iMVPs > pMVPs)
                            {
                                if (plr.isplaying)
                                {
                                    plr.StopAudioAsync();
                                }
                                pMVPs = iMVPs;
                                plr.PlayAudioAsync(selectedAudio["mv"]);
                                
                            }
                            Thread.Sleep(10000);
                            ftimeo = false;

                        }
                        if (planted && notstarted)
                        {
                            if (plr.isplaying)
                            {
                                plr.StopAudioAsync();
                            }
                            notstarted = false;
                            plr.PlayAudioAsync(selectedAudio["bombplanted"]);
                        }
                    }
                    if (RoundEndReason == -1 && !oonce)
                    {
                        if (plr.isplaying)
                        {
                            plr.StopAudioAsync();
                        }
                        plr.PlayAudioAsync(selectedAudio["mainmenu"]);
                        oonce = true;
                    }
                    if (RoundEndReason == -1 && oonce && !plr.isplaying)
                    {
                        plr.PlayAudioAsync(selectedAudio["mainmenu"]);
                    }

                    if (RoundEndReason == 16 && plr.isplaying)
                    {
                        plr.StopAudioAsync();
                        oonce = false;
                    }


                    

                }
                Console.WriteLine("Have a Wonderful Day! Also if you make a music kit, please share it with everyone at this programs thread in unknowncheats");
            }
        }
    }
    class AudioPlayer
    {
        private IWavePlayer wavePlayer;
        private AudioFileReader audioFile;
        public bool isplaying = false;
        // Function to play audio asynchronously
        public async Task PlayAudioAsync(string filePath)
        {
            if (!isplaying) { 
            isplaying = true;
            await Task.Run(() =>
            {
                wavePlayer = new WaveOutEvent();
                audioFile = new AudioFileReader(filePath);

                wavePlayer.Init(audioFile);
                wavePlayer.Play();
                


                // Block the thread until the audio finishes playing
                while (wavePlayer.PlaybackState == PlaybackState.Playing)
                {
                    Task.Delay(100).Wait();
                }
                isplaying = false;
            });
            }
        }

        // Function to stop the audio playback asynchronously
        public async Task StopAudioAsync()
        {
            if (isplaying)
            {
                await Task.Run(() =>
            {
                if (wavePlayer != null)
                {
                    wavePlayer.Stop();
                    wavePlayer.Dispose();
                }

                if (audioFile != null)
                {
                    audioFile.Dispose();
                }

                // You can add additional logic or events here if needed
                Console.WriteLine("Audio stopped.");
                isplaying = false;
            });
            }
        }
    }
}
        