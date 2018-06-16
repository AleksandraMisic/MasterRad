using System.Runtime.Serialization;

namespace DMSCommon.Model
{
    [DataContract]
    [KnownType(typeof(ACLine))]
    [KnownType(typeof(Switch))]
    [KnownType(typeof(Node))]
    [KnownType(typeof(Source))]
    [KnownType(typeof(Consumer))]
    [KnownType(typeof(Node))]
    public class Element
    {
        private long elementGID;
        private bool isEnergized;
        private bool underSCADA;
        private bool incident;
        private string mRID;

        [DataMember]
        public long ElementGID
        {
            get { return elementGID; }
            set { elementGID = value; }
        }

        [DataMember]
        public bool IsEnergized
        {
            get { return isEnergized; }
            set { isEnergized = value; }
        }

        [DataMember]
        public bool UnderSCADA
        {
            get { return underSCADA; }
            set { underSCADA = value; }
        }

        [DataMember]
        public bool Incident
        {
            get { return incident; }
            set { incident = value; }
        }

        [DataMember]
        public string MRID
        {
            get { return mRID; }
            set { mRID = value; }
        }

        public Element() { }

        public Element(long gid)
        {
            ElementGID = gid;
            IsEnergized = true;
        }

        public Element(long gid, string mrid)
        {
            ElementGID = gid;
            MRID = mrid;
            IsEnergized = true;
        }
    }
}
