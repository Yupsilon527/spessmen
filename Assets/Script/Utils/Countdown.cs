using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class Countdown
    {
        float time;
        public bool IsRunning()
        {
            return Time.time < time;
        }
        public float GetTimeRemaining()
        {
            return time - Time.time;
        }
        public void Unique(float dur)
        {
            if (!IsRunning())
            {
                time = Time.time + dur;
            }
        }
        public void Override(float dur)
        {
            time = Time.time + dur;
        }
        public void Extend(float dur)
        {
            time = Mathf.Max(time + dur, Time.time + dur);
        }
        public void Shorten(float dur)
        {
            time -= dur;
        }
        public void Stop()
        {
            time = -1;
        }

}
