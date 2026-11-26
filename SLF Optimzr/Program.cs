using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SLF_Optimzr
{
    internal static class Banner
    {
        private const int WipeDelayMs = 12;
        private const int TypeDelayMs = 8;
        private const int StepDelayMs = 25;
        private const int BarWidth = 28;
        private const string Tagline = "street league football // optimzr";

        private static readonly string[] Logo =
        {
        @" ____   _      _____ ",
        @"/ ___| | |    |  ___|",
        @"\___ \ | |    | |_   ",
        @" ___) || |___ |  _|  ",
        @"|____/ |_____||_|    ",
    };

        private static readonly ConsoleColor[] RowColors =
        {
        ConsoleColor.Cyan, ConsoleColor.Cyan, ConsoleColor.DarkCyan,
        ConsoleColor.DarkCyan, ConsoleColor.DarkGray,
    };

        private static readonly string[] Steps = { "priority", "timer", "settings", "ready" };

        public static void Play()
        {
            if (Console.IsOutputRedirected) { Console.WriteLine("SLF Optimzr"); return; }
            try
            {
                Console.CursorVisible = false;
                Wipe();
                Type();
                Bar();
            }
            catch (Exception)
            {
                // window too small or no real console: plain fallback, never block startup
                Console.ResetColor();
                Console.WriteLine();
                foreach (string row in Logo) Console.WriteLine(row);
            }
            finally
            {
                Console.ResetColor();
                try { Console.CursorVisible = true; } catch (Exception) { }
            }
        }

        // logo wipes in left to right, one column at a time
        private static void Wipe()
        {
            Console.WriteLine();
            for (int r = 0; r < Logo.Length; r++) Console.WriteLine();
            int top = Console.CursorTop - Logo.Length;
            int width = Logo[0].Length;
            for (int col = 0; col < width; col++)
            {
                for (int r = 0; r < Logo.Length; r++)
                {
                    Console.SetCursorPosition(col, top + r);
                    Console.ForegroundColor = RowColors[r];
                    Console.Write(Logo[r][col]);
                }
                Thread.Sleep(WipeDelayMs);
            }
            Console.SetCursorPosition(0, top + Logo.Length);
        }

        private static void Type()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("  ");
            foreach (char c in Tagline)
            {
                Console.Write(c);
                Thread.Sleep(TypeDelayMs);
            }
            Console.WriteLine();
        }

        // cosmetic loading bar, it does not do any work
        private static void Bar()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            int filled = 0;
            for (int s = 0; s < Steps.Length; s++)
            {
                int target = BarWidth * (s + 1) / Steps.Length;
                for (; filled < target; filled++)
                {
                    Console.Write("\r  [" + new string('#', filled + 1)
                        + new string('-', BarWidth - filled - 1) + "] " + Steps[s].PadRight(10));
                    Thread.Sleep(StepDelayMs);
                }
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }
    internal class Program
    {
        // Dont change your game name, which i dont know why you would but yea.
        private const string GameName = "StreetLeagueFootball";
        private const string SettingsFold = "Street League Football";
        private const int PollMS = 2000;
        private const uint TimerPeriodMS = 1;
        // Process Power Throttling
        private const int PPT = 4;
        // Throttle Execution Speed
        private const uint TES = 1;
        private const string exefile = GameName + ".exe";
        private const string GameArgs = "--disable-vsync";
        private const int SearchDepth = 3;
        private const string SavedPathFile = "game-path.txt";
        // Process Query Limited
        private const uint PQL = 0x1000;


        private static readonly Dictionary<string, string> LeanVideo = new Dictionary<string, string>
       {
       { "profile",           "\"custom\"" },
       { "custom_base",       "\"potato\"" },
       { "custom_detail",     "\"potato\"" },
       { "custom_shadows",    "\"off\"" },
       { "custom_msaa",       "0" },
       { "custom_resolution", "\"0.7\"" },
       { "custom_ssao",       "false" },
       { "custom_glow",       "false" },
       { "custom_reflections","false" },
       { "custom_scuffs",     "false" },
       { "custom_board",      "false" },
       { "custom_attract",    "false" },
       { "show_fps",          "true" },
       };
        private static string Launch()
        {
            string exe = FindGameExe();
            if (exe == null) return exefile + " not found";
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(exe, GameArgs)
                {
                    WorkingDirectory = Path.GetDirectoryName(exe),
                    UseShellExecute = false,
                };
                Process started = Process.Start(psi);
                if (started != null) started.Dispose();
                return null;
            }
            catch (Exception e) { return e.Message; }
        }
        private static string VsyncOverride()
        {
            string exe = FindGameExe();
            if (exe == null) return exefile + " not found";
            try
            {
                string cfg = Path.Combine(Path.GetDirectoryName(exe), "override.cfg");
                if (File.Exists(cfg))
                {
                    File.Copy(cfg, cfg + ".bak", true);
                    List<string> lines = File.ReadAllLines(cfg).ToList();
                    int at = lines.FindIndex(l => l.StartsWith("window/vsync/vsync_mode", StringComparison.Ordinal));
                    if (at >= 0) lines[at] = "window/vsync/vsync_mode=0";
                    else { lines.Add(""); lines.Add("[display]"); lines.Add(""); lines.Add("window/vsync/vsync_mode=0"); }
                    File.WriteAllLines(cfg, lines);
                }
                else File.WriteAllText(cfg, "[display]\r\n\r\nwindow/vsync/vsync_mode=0\r\n");
                return null;
            }
            catch (Exception e) { return e.Message; } 
        }

        private static string FindGameExe()
        {
            string saved = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SavedPathFile);
            if (File.Exists(saved))
            {
                string path = File.ReadAllText(saved).Trim();
                if (File.Exists(path)) return path;
            }

            string found = FromRunningProcess();
            if ( found == null)
            {
                foreach (string root in SearchRoots())
                {
                    found = SearchFolder(root, 0);
                    if (found != null) break;
                }
            }
            if (found == null) found = AskForPath();
            if (found != null) File.WriteAllText(saved, found);
            return found;
        }

     

        private static string FromRunningProcess()
        {
            foreach (Process p in Process.GetProcessesByName(GameName))
            {
                using (p)
                {
                    string path = ImagePath(p.Id);
                    if (path != null) return path;
                }
            }
            return null;
        }

        private static string ImagePath(int pid)
        {
            IntPtr h = NativeMethods.OpenProcess(PQL, false, pid);
            if (h == IntPtr.Zero) return null;
            try
            {
                StringBuilder sb = new StringBuilder(1024);
                int size = sb.Capacity;
                return NativeMethods.QueryFullProcessImageName(h, 0, sb, ref size) ? sb.ToString() : null;
            }
            finally { NativeMethods.CloseHandle(h); }
        }
        
        private static IEnumerable<string> SearchRoots()
        {
            yield return AppDomain.CurrentDomain.BaseDirectory;

            yield return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            yield return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            
            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                if (d.DriveType != DriveType.Fixed || !d.IsReady) continue;
                yield return Path.Combine(
                     Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs");
            }
        }


   
        private static string SearchFolder(string dir, int depth)
        {
            try
            {
                string candidate = Path.Combine(dir, exefile);
                if (File.Exists(candidate)) return candidate;
                if (depth >= SearchDepth) return null;
                foreach (string sub in Directory.EnumerateDirectories(dir))
                {
                    string hit = SearchFolder(sub, depth + 1);
                    if (hit != null) return hit;
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException) { }
            return null;
        }

        private static string AskForPath()
        {
            Console.Write("couldnt find " + exefile + "Paste the full path e.g C:\\Users\\(Name)\\Downloads\\slf");
            string input = (Console.ReadLine() ?? "").Trim().Trim('"');
            return File.Exists(input) ? input : null;
        }
        static int Main(string[] args)
        {
            if (!args.Contains("--no-banner")) Banner.Play();
            if (args.Contains("--write-settings"))
            {
                string patchError = PatchSettings(SettingsPath(), "video", LeanVideo);
                Console.WriteLine(patchError ?? "settings patch saved as .bak");
                return patchError == null ? 0 : 1;
            }
            if (args.Contains("--vsync-off"))
            {
                string vsyncError = VsyncOverride();
                Console.WriteLine(vsyncError ?? "vsync off written to override.cfg");
                return vsyncError == null ? 0 : 1;
            }

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) => { e.Cancel = true; cts.Cancel(); };

            NativeMethods.timeBeginPeriod(TimerPeriodMS);
            HashSet<int> tuned = new HashSet<int>();
            try
            {
                Console.WriteLine("waiting for game.... ctrl + c to exit!");
                while (!cts.IsCancellationRequested)
                {
                    Process[] running = Process.GetProcessesByName(GameName);
                    HashSet<int> ids = new HashSet<int>(running.Select(p => p.Id));

                    foreach (Process p in running)
                    {
                        if (!tuned.Add(p.Id)) continue;
                        Console.WriteLine(Tune(p) ?? "tuned pid " + p.Id);
                    }
                    tuned.IntersectWith(ids); 
                    foreach (Process p in running) p.Dispose();

                    cts.Token.WaitHandle.WaitOne(PollMS);
                }
            }
            finally
            {
                NativeMethods.timeEndPeriod(TimerPeriodMS); 
                cts.Dispose();
            }
            return 0;
        }
        private static string Tune(Process p)
        {
            try
            {
                p.PriorityClass = ProcessPriorityClass.High;

                ProcessPowerThrottlingState state = new ProcessPowerThrottlingState
                {
                    Version = 1,
                    ControlMask = TES,
                    StateMask = 0,
                };
                bool ok = NativeMethods.SetProcessInformation(
                    p.Handle, PPT, ref state,
                    (uint)Marshal.SizeOf(typeof(ProcessPowerThrottlingState)));
                return ok ? null : "PT (power throttling) failed, win32 error " +Marshal.GetLastWin32Error();

            }
            catch (Exception e) { return e.Message; }
        }


        
        private static string SettingsPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Godot", "app_userdata", SettingsFold, "settings.cfg");
        }
     

        private static string PatchSettings(string path, string section, Dictionary<string, string> values)
        {
            if (!File.Exists(path)) return "not found: " + path;
            if (Process.GetProcessesByName(GameName).Length > 0)
                return "close the game first, it rewrites settings.cfg on exit";
            File.Copy(path, path + ".bak", true);
            List<string> lines = File.ReadAllLines(path).ToList();
            int start = lines.FindIndex(l => l.Trim() == "[" + section + "]");
            if (start > 0)
            {
                lines.Add("");
                lines.Add("[" + section + "]");
                start = lines.Count - 1;
            }
            int end = start + 1;
            while (end < lines.Count && !lines[end].TrimStart().StartsWith("[", StringComparison.Ordinal)) end++;

            foreach (KeyValuePair<string, string> kv in values)
            {
                string line = kv.Key + "=" + kv.Value;
                int at = lines.FindIndex(start + 1, end  - start -1,
                    l => l.StartsWith(kv.Key +  "=", StringComparison.Ordinal));
                if (at >= 0) lines[at] = line;
                else { lines.Insert(end, line); end++; }
            }
            File.WriteAllLines(path, lines);
            return null;

        }
    
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ProcessPowerThrottlingState
    {
        public uint Version;
        public uint ControlMask;
        public uint StateMask;
    }

    internal static class NativeMethods 
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]

        public static extern bool SetProcessInformation(IntPtr process, int infoClass,
            ref ProcessPowerThrottlingState info, uint size);
        [DllImport("winmm.dll")] public static extern uint timeBeginPeriod(uint ms);
        [DllImport("winmm.dll")] public static extern uint timeEndPeriod(uint ms);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint access, bool inherit, int pid);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryFullProcessImageName(IntPtr process, int flags, StringBuilder exeName, ref int size);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr handle);

    }

}

