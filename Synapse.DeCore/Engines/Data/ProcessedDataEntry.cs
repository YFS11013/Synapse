﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inlite.ClearImageNet;
using Synapse.Core.Configurations;
using Synapse.DeCore.Engines.Data;

namespace Synapse.Core.Engines.Data
{
    [Serializable]
    public struct ProcessedDataEntry
    {
        #region Objects
        [Serializable]
        public class SpecialCell
        {
            public SpecialCell((int entryIndex, int fieldIndex) cell, Color cellBackColor, Color cellForeColor)
            {
                this.cell = cell;
                this.cellBackColor = cellBackColor;
                this.cellForeColor = cellForeColor;
            }

            public (int entryIndex, int fieldIndex) cell { get; set; }
            public Color cellBackColor { get; set; }
            public Color cellForeColor { get; set; }
        }
        #endregion
        #region Properties
        public ConfigurationBase GetConfigurationBase { get => Communicator.GetConfigurationBase?.Invoke(ConfigurationTitle); }
        public string ConfigurationTitle { get; private set; }
        public MainConfigType GetMainConfigType { get => GetConfigurationBase.GetMainConfigType; }
        public ProcessedDataType[] DataEntriesResultType { get; set; }
        public byte[,] GetOptionsOutputs { get => optionsOutputs; }
        private byte[,] optionsOutputs;

        public char[] GetFieldsOutputs { get => fieldsOutputs; set { fieldsOutputs = value; } }
        private char[] fieldsOutputs;
        public string[] GetDataValues { get => dataValues; set { dataValues = value; } }
        private string[] dataValues;

        public List<SpecialCell> SpecialCells;
        public Barcode[] BarcodesResult;

        public bool IsEdited { get; set; }
        #endregion


        #region Methods
        public ProcessedDataEntry(string configurationTitle, char[] fieldsOutputs, ProcessedDataType[] processedDataResultType, byte[,] optionsOutputs, Barcode[] barcodesResult = null)
        {
            this.ConfigurationTitle = configurationTitle;
            this.fieldsOutputs = fieldsOutputs;
            this.optionsOutputs = optionsOutputs;

            DataEntriesResultType = processedDataResultType;
            dataValues = null;
            IsEdited = false;
            SpecialCells = new List<SpecialCell>();

            BarcodesResult = barcodesResult;
            FormatData();
        }
        public ProcessedDataType? GetRegionDataType()
        {
            ProcessedDataType? result = null;

            if (GetMainConfigType == MainConfigType.OMR)
            {
                OMRConfiguration omrConfiguration = (OMRConfiguration)GetConfigurationBase;
                switch (omrConfiguration.ValueRepresentation)
                {
                    case ValueRepresentation.Collective:
                        result = DataEntriesResultType.Contains(ProcessedDataType.INCOMPATIBLE) ? ProcessedDataType.INCOMPATIBLE : DataEntriesResultType.Contains(ProcessedDataType.FAULTY) ? ProcessedDataType.FAULTY : DataEntriesResultType.Contains(ProcessedDataType.MANUAL) ? ProcessedDataType.MANUAL : ProcessedDataType.NORMAL;
                        break;
                    case ValueRepresentation.Indiviual:
                        result = null;
                        break;
                    case ValueRepresentation.CombineTwo:
                        result = null;
                        break;
                    default:
                        break;
                }
            }
            else
                result = DataEntriesResultType.Contains(ProcessedDataType.INCOMPATIBLE) ? ProcessedDataType.INCOMPATIBLE : DataEntriesResultType.Contains(ProcessedDataType.FAULTY) ? ProcessedDataType.FAULTY : DataEntriesResultType.Contains(ProcessedDataType.MANUAL) ? ProcessedDataType.MANUAL : ProcessedDataType.NORMAL;

            return result;
        }
        public ProcessedDataType? GetRowDataType()
        {
            ProcessedDataType? result = null;

            if(GetMainConfigType == MainConfigType.OMR)
            {
                OMRConfiguration omrConfiguration = (OMRConfiguration)GetConfigurationBase;
                switch (omrConfiguration.ValueRepresentation)
                {
                    case ValueRepresentation.Collective:
                        result = DataEntriesResultType.Contains(ProcessedDataType.INCOMPATIBLE) ? ProcessedDataType.INCOMPATIBLE : DataEntriesResultType.Contains(ProcessedDataType.FAULTY) ? ProcessedDataType.FAULTY : DataEntriesResultType.Contains(ProcessedDataType.MANUAL) ? ProcessedDataType.MANUAL : ProcessedDataType.NORMAL;
                        break;
                    case ValueRepresentation.Indiviual:
                        result = DataEntriesResultType.Contains(ProcessedDataType.INCOMPATIBLE) ? ProcessedDataType.INCOMPATIBLE : DataEntriesResultType.Contains(ProcessedDataType.FAULTY) ? ProcessedDataType.FAULTY : DataEntriesResultType.Contains(ProcessedDataType.MANUAL) ? ProcessedDataType.MANUAL : ProcessedDataType.NORMAL;
                        break;
                    case ValueRepresentation.CombineTwo:
                        result = DataEntriesResultType.Contains(ProcessedDataType.INCOMPATIBLE) ? ProcessedDataType.INCOMPATIBLE : DataEntriesResultType.Contains(ProcessedDataType.FAULTY) ? ProcessedDataType.FAULTY : DataEntriesResultType.Contains(ProcessedDataType.MANUAL) ? ProcessedDataType.MANUAL : ProcessedDataType.NORMAL;
                        break;
                    default:
                        break;
                }
            }
            else
                result = DataEntriesResultType.Contains(ProcessedDataType.INCOMPATIBLE) ? ProcessedDataType.INCOMPATIBLE : DataEntriesResultType.Contains(ProcessedDataType.FAULTY) ? ProcessedDataType.FAULTY : DataEntriesResultType.Contains(ProcessedDataType.MANUAL) ? ProcessedDataType.MANUAL : ProcessedDataType.NORMAL;

            return result;
        }
        public string[] FormatData()
        {
            var config = GetConfigurationBase;

            string fieldsOutput = new string(fieldsOutputs);

            if (config.GetMainConfigType == MainConfigType.OMR)
            {
                OMRConfiguration configOMR = (OMRConfiguration)config;
                if (configOMR.ImplicitValue)
                {
                    string impliedValue = "";
                    bool startImplied = false;
                    int endRemoveCount = 0;
                    for (int i = 0; i < fieldsOutput.Length; i++)
                    {
                        if (fieldsOutput[i] != configOMR.NoneMarkedSymbol)
                            startImplied = true;
                        else
                            if(!startImplied) DataEntriesResultType[i] = ProcessedDataType.NORMAL;

                        if (startImplied)
                        {
                            impliedValue += fieldsOutput[i];

                            if (fieldsOutput[i] == configOMR.NoneMarkedSymbol)
                                endRemoveCount++;
                            else
                                endRemoveCount = 0;
                        }
                    }
                    for (int i = DataEntriesResultType.Length - endRemoveCount; i < DataEntriesResultType.Length; i++)
                    {
                        DataEntriesResultType[i] = ProcessedDataType.NORMAL;
                    }
                    impliedValue = impliedValue.Remove(impliedValue.Length - endRemoveCount, endRemoveCount);
                    fieldsOutput = impliedValue;
                }
            }

            
            List<string> result = new List<string>();
            switch (config.ValueRepresentation)
            {
                case ValueRepresentation.Collective:
                    result.Add(fieldsOutput);
                    break;
                case ValueRepresentation.Indiviual:
                    for (int i = 0; i < fieldsOutput.Length; i++)
                    {
                        result.Add(fieldsOutput[i] + "");
                    }
                    break;
                case ValueRepresentation.CombineTwo:
                    if (fieldsOutput.Length % 2 == 0)
                    {
                        for (int i = 0; i < fieldsOutput.Length; i += 2)
                        {
                            result.Add(string.Concat(fieldsOutput[i] + fieldsOutput[i + 1]));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < fieldsOutput.Length; i++)
                        {
                            result.Add(fieldsOutput[i] + "");
                        }
                    }
                    break;
            }

            dataValues = result.ToArray();

            return dataValues;
        }
        public static byte[,] GenerateRawOMRDataValues(OMRConfiguration omrConfiguration, char[] fieldsOutputs, char[] escapeChars)
        {
            byte totalFields = (byte)omrConfiguration.GetTotalFields;
            byte totalOptions = (byte)omrConfiguration.GetTotalOptions;

            byte[,] result = new byte[totalFields, totalOptions];

            var outputType = omrConfiguration.ValueDataType;
            byte[] ascii = Encoding.ASCII.GetBytes(fieldsOutputs);
            byte[] escapeAscii = Encoding.ASCII.GetBytes(escapeChars);
            switch (outputType)
            {
                case ValueDataType.String:
                    break;
                case ValueDataType.Text:
                    break;
                case ValueDataType.Alphabet:
                    for (int i = 0; i < totalFields; i++)
                    {
                        if (escapeAscii.Contains(ascii[i]))
                            continue;

                        if(ascii[i] == 64)
                        {
                            result[i, 0] = 2;
                            continue;
                        }

                        int optionIndex = ascii[i] - 65;
                        result[i, optionIndex] = 1;
                    }
                    break;
                case ValueDataType.WholeNumber:
                    for (int i = 0; i < totalFields; i++)
                    {
                        if (escapeAscii.Contains(ascii[i]))
                            continue;

                        int optionIndex = ascii[i] - 48;
                        result[i, optionIndex] = 1;
                    }
                    break;
                case ValueDataType.NaturalNumber:
                    for (int i = 0; i < totalFields; i++)
                    {
                        if (escapeAscii.Contains(ascii[i]))
                            continue;

                        int optionIndex = ascii[i] - 49;
                        result[i, optionIndex] = 1;
                    }
                    break;
                case ValueDataType.Integer:
                    break;
                default:
                    break;
            }

            return result;
        }
        #endregion
    }
}