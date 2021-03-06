using System;
using System.Collections.Generic;

namespace IctBaden.RasPi
{
    public class Gpio
    {
        Dictionary<uint, uint> ioMode = new Dictionary<uint, uint>();
        private int[] inputAssignment = { 17, 27, 22, 18 };
        private int[] outputAssignment = { 7, 8, 9, 10, 11, 23, 24, 25 };

        /// <summary>
        /// GPIO numbers used as digital inputs.
        /// Call Initialize() after changing this.
        /// </summary>
        public int[] InputAssignment
        {
            get
            {
                return inputAssignment;
            }
            set
            {
                inputAssignment = value;
            }
        }

        /// <summary>
        /// GPIO numbers used as digital outputs.
        /// Call Initialize() after changing this.
        /// </summary>
        public int[] OutputAssignment
        {
            get
            {
                return outputAssignment;
            }
            set
            {
                outputAssignment = value;
            }
        }

        /// <summary>
        /// GPIO numbers and I/O ALT-mode to use with
        /// Call Initialize() after changing this.
        /// </summary>
        public Dictionary<uint, uint> IoMode
        {
            get
            {
                return ioMode;
            }
            set
            {
                ioMode = value;
            }
        }

        public int Inputs { get { return inputAssignment.Length; } }

        public int Outputs { get { return outputAssignment.Length; } }

        public bool Initialize()
        {
            try
            {
                RawGpio.Initialize();
            } catch (Exception)
            {
                return false;
            }

            foreach (var mode in ioMode)
            {
                RawGpio.INP_GPIO(mode.Key);
                RawGpio.SET_GPIO_ALT(mode.Key, mode.Value);
            }

            foreach (var input in inputAssignment)
            {
                RawGpio.INP_GPIO((uint)input);
            }
            
            foreach (var output in outputAssignment)
            {
                RawGpio.INP_GPIO((uint)output); // must use INP_GPIO before we can use OUT_GPIO
                RawGpio.OUT_GPIO((uint)output);
            }
            
            return true;
        }

        public void SetOutput(int index, bool value)
        {
            if ((index < 0) || (index >= Outputs))
            {
                throw new ArgumentException("Output out of range", "index");
            }
            if (value)
            {
                RawGpio.GPIO_SET = (uint)(1 << outputAssignment [index]);
            } else
            {
                RawGpio.GPIO_CLR = (uint)(1 << outputAssignment [index]);
            }
        }

        public bool GetInput(int index)
        {
            if ((index < 0) || (index >= Inputs))
            {
                throw new ArgumentException("Input out of range", "index");
            }

            return (RawGpio.GPIO_IN0 & (uint)(1 << inputAssignment [index])) != 0;
        }

        public ulong GetInputs()
        {
            ulong inputs = 0;
            for (var ix = 0; ix < Inputs; ix++)
            {
                if (GetInput(ix))
                {
                    inputs |= (ulong)1 << ix;
                }
            }
            return inputs;
        }
    }
}
