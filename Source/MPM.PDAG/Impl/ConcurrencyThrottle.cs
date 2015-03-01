﻿#region HEADER
//   Copyright 2015 Myles McDonnell (mcdonnell.myles@gmail.com)

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//     http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
#endregion

using System;
using System.Threading;

namespace MPM.PDAG
{
    public class ConcurrencyThrottle : IConcurrencyThrottle
    {
        public bool Enabled { get; set; }
        public int MaxValue { get; set; }
        public int CurrentValue { get; private set; }
        
        private readonly object _lock = new object();

        public ConcurrencyThrottle()
        {
            MaxValue = Environment.ProcessorCount;
        }

        public void Exit()
        {
            lock(_lock)
            {
                CurrentValue--;

                Monitor.PulseAll(_lock);
            }
        }

        public void Enter()
        {
            lock(_lock)
            {
                while (Enabled && CurrentValue >= MaxValue)
                {
                    Monitor.Wait(_lock);
                }

                MaxValue = Math.Max(++CurrentValue, MaxValue);
            }
        }
    }
}
