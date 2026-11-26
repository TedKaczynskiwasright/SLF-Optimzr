using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using System.Net.NetworkInformation;

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
        private const string LayersKey = @"Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers";
        private const string FsoToken = "DISABLEDXMAXIMIZEDWINDOWEDMODE";
        private const uint TimerPeriodMS = 1;
        private const string ProfileKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";
        private const int PingCount = 20;
        private const int PingTimeoutMs = 1000;
        private const string Absent = "<absent>";
        // Process Power Throttling
        private const int PPT = 4;
        // Throttle Execution Speed
        private const uint TES = 1;
        private const string exefile = GameName + ".exe";
        private const string GameArgs = "--disable-vsync";
        private const int SearchDepth = 3;
        private const string SavedPathFile = "game-path.txt";
        private const int AnimMs = 80;
        private const int BannerHoldMs = 700;
        private const int BarSpan = 24;
        // Process Query Limited
        private const uint PQL = 0x1000;
        private const int BarCells = 20;
        private const double Ease = 0.28;
        private const string Cancelled = "cancelled";

        private static readonly Dictionary<string, string> LeanVideo = new Dictionary<string, string>
        {
            { "profile", "\"custom\"" },
            { "custom_base", "\"potato\"" },
            { "custom_detail", "\"potato\"" },
            { "custom_shadows", "\"off\"" },
            { "custom_msaa", "0" },
            { "custom_resolution", "\"0.7\"" },
            { "custom_ssao", "false" },
            { "custom_glow", "false" },
            { "custom_reflections", "false" },
            { "custom_scuffs", "false" },
            { "custom_board", "false" },
            { "custom_attract", "false" },
            { "show_fps", "true" },
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

        private static string BackupPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "net-backup.txt");
        }

        private static void PingTest(string host)
        {
            List<long> rtts = new List<long>();
            using (Ping ping = new Ping())
            {
                for (int i = 0; i < PingCount; i++)
                {
                    try
                    {
                        PingReply r = ping.Send(host, PingTimeoutMs);
                        if (r != null && r.Status == IPStatus.Success) rtts.Add(r.RoundtripTime);
                    }
                    catch (PingException) { }
                    Thread.Sleep(200);
                }
            }
            if (rtts.Count == 0) { Console.WriteLine("  no replies from " + host); return; }
            double jitter = 0;
            for (int i = 1; i < rtts.Count; i++) jitter += Math.Abs(rtts[i] - rtts[i - 1]);
            jitter /= Math.Max(1, rtts.Count - 1);
            Console.WriteLine("  {0}: min {1} avg {2:F0} max {3} jitter {4:F1} ms, {5}/{6} replies",
                host, rtts.Min(), rtts.Average(), rtts.Max(), jitter, rtts.Count, PingCount);
        }

        private static string NetTweaks(bool undo)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(ProfileKey, true))
                {
                    if (key == null) return "registry key missing: " + ProfileKey;
                    if (undo)
                    {
                        if (!File.Exists(BackupPath())) return "no backup file next to the exe";
                        foreach (string line in File.ReadAllLines(BackupPath()))
                        {
                            string[] kv = line.Split('=');
                            if (kv.Length != 2) continue;
                            if (kv[1] == Absent) key.DeleteValue(kv[0], false);
                            else key.SetValue(kv[0], int.Parse(kv[1]), RegistryValueKind.DWord);
                        }
                        return null;
                    }

                    Dictionary<string, int> wanted = new Dictionary<string, int>
                    {
                        { "NetworkThrottlingIndex", unchecked((int)0xFFFFFFFF) },
                        { "SystemResponsiveness", 10 },
                    };
                    List<string> backup = new List<string>();
                    foreach (KeyValuePair<string, int> kv in wanted)
                    {
                        object old = key.GetValue(kv.Key);
                        backup.Add(kv.Key + "=" + (old == null ? Absent : old.ToString()));
                    }
                    if (!File.Exists(BackupPath())) File.WriteAllLines(BackupPath(), backup);
                    foreach (KeyValuePair<string, int> kv in wanted)
                        key.SetValue(kv.Key, kv.Value, RegistryValueKind.DWord);
                    return null;
                }
            }
            catch (UnauthorizedAccessException) { return "run as Administrator"; }
            catch (Exception e) { return e.Message; }
        }

        private static string FullscreenOpt(bool undo)
        {
            string exe = FindGameExe();
            if (exe == null) return exefile + " not found";
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(LayersKey))
                {
                    string old = (key.GetValue(exe) as string ?? "").Replace("~", "").Trim();
                    List<string> flags = old.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    flags.RemoveAll(f => f == FsoToken);
                    if (!undo) flags.Add(FsoToken);

                    if (flags.Count == 0) key.DeleteValue(exe, false);
                    else key.SetValue(exe, "~ " + string.Join(" ", flags), RegistryValueKind.String);
                }
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
                Console.WriteLine("wrote " + cfg);
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
            if (found == null)
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
            if (args.Length == 0) return MenuLoop();
            if (args.Contains("--tune")) { RunTuner(); return 0; }
            if (args.Contains("--vsync-off"))
            {
                string vsyncError = VsyncOverride();
                Console.WriteLine(vsyncError ?? "vsync off written to override.cfg");
                return vsyncError == null ? 0 : 1;
            }
            int pi = Array.IndexOf(args, "--profile");
            if (pi >= 0 && pi + 1 < args.Length)
            {
                ProfileDef chosen = Profiles.FirstOrDefault(
                    x => x.Name.Equals(args[pi + 1], StringComparison.OrdinalIgnoreCase));
                string profileError = chosen == null
                    ? "unknown profile, use potato, balanced or beast"
                    : ApplyProfile(chosen);
                Console.WriteLine(profileError ?? "profile applied");
                return profileError == null ? 0 : 1;
            }
            if (args.Contains("--fso-off") || args.Contains("--fso-undo"))
            {
                string fsoError = FullscreenOpt(args.Contains("--fso-undo"));
                Console.WriteLine(fsoError ?? "done, restart the game");
                return fsoError == null ? 0 : 1;
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

        private static int MenuLoop()
        {
            Thread.Sleep(BannerHoldMs);
            string[] labels =
            {
                "Apply light graphics profile",
                "Vsync off (override.cfg)",
                "Fullscreen optimizations off",
                "Launch game",
                "Start tuner (keep open)",
                "Ping test",
                "Network tweaks on",
                "Network tweaks undo",
                "Quit",
            };
            Func<string>[] actions =
            {
                () => PatchSettings(SettingsPath(), "video", LeanVideo),
                () => VsyncOverride(),
                () => FullscreenOpt(false),
                () => Launch(),
                () => { RunTuner(); return null; },
                () => { PingTest("1.1.1.1"); return null; },
                () => NetTweaks(false),
                () => NetTweaks(true),
                null,
            };

            int sel = 0;
            try
            {
                Console.CursorVisible = false;
                while (true)
                {
                    int pick = Pick(labels, sel);
                    if (pick < 0 || pick == labels.Length - 1) return 0;
                    sel = pick;

                    Console.Clear();
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine("  " + labels[pick] + " ...");
                    Console.WriteLine();
                    string error = actions[pick]();
                    if (error == Cancelled) continue;
                    Console.ForegroundColor = error == null ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine("  " + (error == null ? "done" : "failed: " + error));
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.Write("  press any key to go back");
                    Console.ReadKey(true);
                }
            }
            finally
            {
                Console.ResetColor();
                Console.CursorVisible = true;
            }
        }

        private sealed class ProfileDef
        {
            public string Name;
            public string Blurb;
            public string Preset;
            public int Looks;
            public int Speed;
            public bool Replays;
            public ConsoleColor Accent;
        }

        private static readonly ProfileDef[] Profiles =
        {
            new ProfileDef { Name = "POTATO", Preset = "potato", Looks = 2, Speed = 10, Replays = false,
                Accent = ConsoleColor.Yellow, Blurb = "shadows off, no AA, no extras, 70% render scale" },
            new ProfileDef { Name = "BALANCED", Preset = "performance", Looks = 6, Speed = 7, Replays = true,
                Accent = ConsoleColor.Cyan, Blurb = "low shadows and detail, light AA, automatic render scale" },
            new ProfileDef { Name = "BEAST", Preset = "quality", Looks = 10, Speed = 3, Replays = true,
                Accent = ConsoleColor.Green, Blurb = "high detail and shadows, 2x AA, full resolution" },
            // i cried making this
        };

        private static string ApplyProfile(ProfileDef p)
        {
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                { "graphics_version", "2" },
                { "profile", "\"" + p.Preset + "\"" },
                { "replays", p.Replays ? "true" : "false" },
                { "show_fps", "true" },
            };
            return PatchSettings(SettingsPath(), "video", values);
        }

        private static string ProfilePicker()
        {
            int sel = 1;
            double speed = 0, looks = 0;

            Console.Clear();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  GRAPHICS PROFILE");
            Console.ResetColor();
            const int rows = 9;
            for (int i = 0; i < rows; i++) Console.WriteLine();
            int top = Console.CursorTop - rows;
            int tick = 0;

            while (true)
            {
                ProfileDef p = Profiles[sel];
                speed += (p.Speed - speed) * Ease;
                looks += (p.Looks - looks) * Ease;
                DrawProfiles(top, sel, speed, looks, tick++);

                if (!Console.KeyAvailable) { Thread.Sleep(AnimMs); continue; }
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.LeftArrow) sel = (sel + Profiles.Length - 1) % Profiles.Length;
                else if (key == ConsoleKey.RightArrow) sel = (sel + 1) % Profiles.Length;
                else if (key == ConsoleKey.Escape) return Cancelled;
                else if (key == ConsoleKey.Enter)
                {
                    for (int f = 0; f < 10; f++)
                    {
                        DrawProfiles(top, sel, speed, looks, f * 5);
                        Thread.Sleep(AnimMs / 2);
                    }
                    return ApplyProfile(p);
                }
            }
        }
        private static void DrawProfiles(int top, int sel, double speed, double looks, int tick)
        {
            ProfileDef p = Profiles[sel];
            Console.SetCursorPosition(0, top);
            Console.Write("  ");
            for (int i = 0; i < Profiles.Length; i++)
            {
                Console.ResetColor();
                if (i == sel)
                {
                    Console.BackgroundColor = (tick / 5) % 2 == 0 ? Profiles[i].Accent : ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" " + Profiles[i].Name.PadRight(8) + " ");
                Console.ResetColor();
                Console.Write("  ");
            }

            Console.SetCursorPosition(0, top + 2);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(("  " + p.Blurb).PadRight(78));

            DrawBar(top + 4, "SPEED", speed, p.Accent);
            DrawBar(top + 5, "LOOKS", looks, p.Accent);

            Console.SetCursorPosition(0, top + 7);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("  left/right switch, enter apply, esc back".PadRight(78));
            Console.ResetColor();
        }

        private static void DrawBar(int row, string label, double value, ConsoleColor color)
        {
            int filled = (int)Math.Round(value / 10.0 * BarCells);
            filled = Math.Max(0, Math.Min(BarCells, filled));
            Console.SetCursorPosition(0, row);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("  " + label + "  [");
            Console.ForegroundColor = color;
            Console.Write(new string('#', filled));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('-', BarCells - filled));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("]  " + value.ToString("0.0").PadLeft(4) + "   ");
            Console.ResetColor();
        }

        private static int Pick(string[] labels, int sel)
        {
            Console.Clear();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  SLF OPTIMZR");
            Console.ResetColor();
            int rows = labels.Length + 4;
            for (int i = 0; i < rows; i++) Console.WriteLine();
            int top = Console.CursorTop - rows;
            int width = labels.Max(l => l.Length);
            int tick = 0;

            while (true)
            {
                DrawMenu(top, labels, sel, tick++, width);
                if (!Console.KeyAvailable) { Thread.Sleep(AnimMs); continue; }
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow) sel = (sel + labels.Length - 1) % labels.Length;
                else if (key == ConsoleKey.DownArrow) sel = (sel + 1) % labels.Length;
                else if (key == ConsoleKey.Enter) return sel;
                else if (key == ConsoleKey.Escape) return -1;
            }
        }

        private static void DrawMenu(int top, string[] labels, int sel, int tick, int width)
        {
            int cycle = BarSpan * 2 - 2;
            int pos = tick % cycle;
            if (pos >= BarSpan) pos = cycle - pos;
            char[] bar = new string('-', BarSpan).ToCharArray();
            bar[pos] = '#';

            Console.SetCursorPosition(0, top);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("  [" + new string(bar) + "]  up/down move, enter select, esc quit  ");

            for (int i = 0; i < labels.Length; i++)
            {
                Console.SetCursorPosition(0, top + 2 + i);
                bool on = i == sel;
                Console.ResetColor();
                Console.Write("  ");
                if (on)
                {
                    Console.BackgroundColor = (tick / 5) % 2 == 0 ? ConsoleColor.Cyan : ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write((on ? ">> [ " : "   [ ") + labels[i].PadRight(width) + " ]");
                Console.ResetColor();
                Console.Write("  ");
            }
            Console.ResetColor();
        }

        private static void RunTuner()
        {
            NativeMethods.timeBeginPeriod(TimerPeriodMS);
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                ConsoleCancelEventHandler stop = (s, e) => { e.Cancel = true; cts.Cancel(); };
                Console.CancelKeyPress += stop;
                HashSet<int> tuned = new HashSet<int>();
                string[] spin = { "|", "/", "-", "\\" };
                Stopwatch clock = Stopwatch.StartNew();
                long nextPoll = 0;
                int frame = 0;
                string note = "";
                try
                {
                    Console.WriteLine("  tuner running, ctrl+c to go back");
                    Console.WriteLine();
                    int row = Console.CursorTop;
                    while (!cts.IsCancellationRequested)
                    {
                        if (clock.ElapsedMilliseconds >= nextPoll)
                        {
                            nextPoll = clock.ElapsedMilliseconds + PollMS;
                            Process[] running = Process.GetProcessesByName(GameName);
                            HashSet<int> ids = new HashSet<int>(running.Select(p => p.Id));
                            foreach (Process p in running)
                            {
                                if (!tuned.Add(p.Id)) continue;
                                string err = Tune(p);
                                note = err ?? "tuned pid " + p.Id;
                            }
                            tuned.IntersectWith(ids);
                            foreach (Process p in running) p.Dispose();
                            if (tuned.Count == 0) note = "";
                        }
                        Console.SetCursorPosition(0, row);
                        Console.ForegroundColor = tuned.Count > 0 ? ConsoleColor.Green : ConsoleColor.Yellow;
                        Console.Write("  " + spin[frame++ % spin.Length] + " "
                            + (tuned.Count > 0 ? "game tuned  " + note : "waiting for the game").PadRight(50));
                        Console.ResetColor();
                        cts.Token.WaitHandle.WaitOne(AnimMs);
                    }
                }
                finally
                {
                    Console.CancelKeyPress -= stop;
                    NativeMethods.timeEndPeriod(TimerPeriodMS);
                }
            }
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
                return ok ? null : "PT (power throttling) failed, win32 error " + Marshal.GetLastWin32Error();
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
            if (start < 0)
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
                int at = lines.FindIndex(start + 1, end - start - 1,
                    l => l.StartsWith(kv.Key + "=", StringComparison.Ordinal));
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

        [DllImport("winmm.dll")]
        public static extern uint timeBeginPeriod(uint ms);

        [DllImport("winmm.dll")]
        public static extern uint timeEndPeriod(uint ms);

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