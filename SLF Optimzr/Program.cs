using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace SLF_Optimzr
{
    internal class Program
    {
        // Dont change your game name, which i dont know why you would but yea.
        private const string GameName = "Street League Football";
        private const string SettingsFold = "Street League Football";
        private const int PollMS = 2000;
        private const uint TimerPeriodMS = 1;
        // Process Power Throttling
        private const int PPT = 4;
        // Throttle Execution Speed
        private const uint TES = 1;

        private static readonly Dictionary<string, string> LeanVideo = new Dictionary<string, string>
        {
            { "profile",    "\"custom\"" },
            { "custom base", "\"perfomance\"" },
            { "custom detail", "\"low\"" },
            { "custom shadows", "\"minimal\"" },
            { "custom msaa", "0" },
            { "custom resolution", "\"0.85\""},
            { "custom ssao", "false" },
            { "custom glow", "false" },
            { "custom scuffs", "false" },
            { "custom attract", "false" },
            { "show fps", "true" },
        };

        static int Main(string[] args)
        {    
            if (args.Contains("--write-settings"))
            {
                string patchError = PatchSettings(SettingsPath(), "video", LeanVideo);
                Console.WriteLine(patchError ?? "settings patch saved as .bak");
                return patchError == null ? 0 : 1;
            }

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) => { e.Cancel = true; cts.Cancel(); };

            NativeMethods.timeBeginPeriod(TimerPeriodMS);
            HashSet<int> tuned = new HashSet<int>();
            try
            {
                Console.WriteLine("waiting for game.... ctrl + to exit!");
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

    }

}

