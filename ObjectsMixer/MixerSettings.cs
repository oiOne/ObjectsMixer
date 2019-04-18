using System.Dynamic;

namespace ObjectsMixer
{
    public class MixerSettings
    {
        private Priority _priority = Priority.Merge;
        public Priority Priority => _priority;
        public MixerSettings WithLeftPriority()
        {
            _priority = Priority.Left;
            return this;
        }
        public MixerSettings WithRightPriority()
        {
            _priority = Priority.Right;
            return this;
        }

        public ExpandoObject Mix(object left, object right)
        {
            return new Mixer().MixObjects(left, right, this);
        }
    }
}