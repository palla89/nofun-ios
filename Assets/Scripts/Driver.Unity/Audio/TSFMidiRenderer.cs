/*
 * (C) 2023 Radrat Softworks
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Runtime.InteropServices;

namespace Nofun.Driver.Unity.Audio
{
    public static class TSFMidiRenderer
    {
#if UNITY_IOS && !UNITY_EDITOR
        // On iOS the renderer is compiled from source into the app binary
        private const string LibraryName = "__Internal";
#else
        private const string LibraryName = "TSFMidiRenderer";
#endif

        [DllImport(LibraryName, EntryPoint = "nofunTSFStartup")]
        public static extern int Startup(IntPtr soundFontBuffer, uint soundFontSize, int outputFreq);

        [DllImport(LibraryName, EntryPoint = "nofunTSFShutdown")]
        public static extern void Shutdown();

        [DllImport(LibraryName, EntryPoint = "nofunTSFLoad")]
        public static extern IntPtr Load(IntPtr midiData, uint midiDataSize, int loop = 0);

        [DllImport(LibraryName, EntryPoint = "nofunTSFFree")]
        public static extern void Free(IntPtr handle);

        [DllImport(LibraryName, EntryPoint = "nofunTSFGetBuffer")]
        public static extern void GetBuffer(IntPtr dataBuffer, int sampleCount);

        [DllImport(LibraryName, EntryPoint = "nofunTSFGetDonePlayingHandles")]
        public static extern IntPtr GetDonePlayingHandles(IntPtr count);
    };
}