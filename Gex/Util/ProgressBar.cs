using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gex.Util
{
    public class ProgressBar
    {
        public float current, duration;

        public ProgressBar(float duration)
        {
            current = 0f;
            this.duration = duration;
        }

        public float Progress
        {
            get { return current / duration; }
        }

        public bool Done
        {
            get { return current / duration >= 1f; }
        }

        public void Reset()
        {
            current = 0;
        }

        public void Update(float time)
        {
            current += time;
            if (current > duration)
            {
                current = duration;
            }
        }
    }
}
