﻿namespace AngleSharp.DOM.Css.Properties
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// More information available at:
    /// https://developer.mozilla.org/en-US/docs/CSS/transition
    /// </summary>
    sealed class CSSTransitionProperty : CSSProperty, ICssTransitionProperty
    {
        #region Fields

        List<Transition> _transitions;

        #endregion

        #region ctor

        internal CSSTransitionProperty()
            : base(PropertyNames.Transition)
        {
            _transitions = new List<Transition>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the durations for the transitions.
        /// </summary>
        public IEnumerable<Time> Durations
        {
            get
            {
                foreach (var time in _transitions)
                    yield return time.Duration;
            }
        }

        /// <summary>
        /// Gets the offsets for the transitions.
        /// </summary>
        public IEnumerable<Time> Delays
        {
            get
            {
                foreach (var time in _transitions)
                    yield return time.Delay;
            }
        }

        /// <summary>
        /// Gets the timing-functions for the transitions.
        /// </summary>
        internal IEnumerable<CSSTimingValue> TimingFunctions
        {
            get
            {
                foreach (var time in _transitions)
                    yield return time.Timing;
            }
        }

        /// <summary>
        /// Gets the properties for the transitions.
        /// </summary>
        public IEnumerable<String> Properties
        {
            get
            {
                foreach (var time in _transitions)
                    yield return time.Property;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines if the given value represents a valid state of this property.
        /// </summary>
        /// <param name="value">The state that should be used.</param>
        /// <returns>True if the state is valid, otherwise false.</returns>
        protected override Boolean IsValid(CSSValue value)
        {
            if (value.Is(Keywords.None))
            {
                _transitions.Clear();
                return true;
            }
            
            var transition = ParseValue(value);

            if (transition.HasValue)
            {
                _transitions.Clear();
                _transitions.Add(transition.Value);
            }
            else if (value is CSSValueList)
            {
                var values = ((CSSValueList)value).ToList();
                var list = new List<Transition>();

                foreach (var item in values)
                {
                    var t = item.Length == 1 ? ParseValue(item[0]) : ParseValue(item);

                    if (!t.HasValue)
                        return false;

                    list.Add(t.Value);
                }

                _transitions = list;
            }
            else if (value != CSSValueList.Inherit)
                return false;

            return true;
        }

        Transition? ParseValue(CSSValue value)
        {
            Time? duration = null;
            var function = value.ToTimingFunction();
            var property = Keywords.All;

            if (function == null)
            {
                function = CSSTimingValue.Ease;

                if (value is CSSIdentifierValue)
                    property = ((CSSIdentifierValue)value).Value;
                else if (!(duration = value.ToTime()).HasValue)
                    return null;
            }

            return new Transition
            {
                Delay = Time.Zero,
                Duration = duration ?? Time.Zero,
                Timing = function,
                Property = property
            };
        }

        Transition? ParseValue(CSSValueList values)
        {
            Time? delay = null;
            Time? duration = null;
            CSSTimingValue function = null;
            String property = null;

            for (var i = 0; i < values.Length; i++)
            {
                if (function == null && (function = values[i].ToTimingFunction()) != null)
                    continue;

                if (property == null && values[i] is CSSIdentifierValue)
                {
                    property = ((CSSIdentifierValue)values[i]).Value;
                    continue;
                }

                var time = values[i].ToTime();

                if (time.HasValue)
                {
                    if (duration == null)
                    {
                        duration = time;
                        continue;
                    }
                    else if (delay == null)
                    {
                        delay = time;
                        continue;
                    }
                }

                return null;
            }

            return new Transition
            {
                Delay = delay ?? Time.Zero,
                Duration = duration ?? Time.Zero,
                Timing = function ?? CSSTimingValue.Ease,
                Property = property ?? Keywords.All
            };
        }

        #endregion

        #region Transition

        struct Transition
        {
            public Time Delay;
            public Time Duration;
            public CSSTimingValue Timing;
            public String Property;
        }

        #endregion
    }
}
