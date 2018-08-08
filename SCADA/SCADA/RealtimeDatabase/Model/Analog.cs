using SCADA.RealtimeDatabase.Catalogs;

namespace SCADA.RealtimeDatabase.Model
{
    public class Analog : ProcessVariable
    {
        public Analog()
        {
            this.Type = VariableTypes.ANALOG;

            this.NumOfRegisters = 1;

            IsInit = false;

            MinValue = 50;
            MaxValue = 500;
        }

        public ushort NumOfRegisters { get; set; }
        
        public UnitSymbol UnitSymbol { get; set; }

        public float AcqValue { get; set; }
        
        public float CommValue { get; set; }
        
        public float MinValue { get; set; }

        public float MaxValue { get; set; }

        public ushort RawAcqValue { get; set; }

        public ushort RawCommValue { get; set; } 

        public ushort RawBandLow { get; set; } 

        public ushort RawBandHigh { get; set; }

        public int Scale { get; set; }

        public float Offset { get; set; }
    }
}
