﻿using Synapse.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.Core.Keys
{
    #region Enums
    public enum KeyType
    {
        [EnumDescription("General")]
        General,
        [EnumDescription("Parameter Based")]
        ParameterBased
    }
    #endregion

    [Serializable]
    internal class AnswerKey
    {
        private AnswerKey key;

        public AnswerKey(AnswerKey key)
        {
            this.key = key;
        }
    }
}
