using System;
using System.Collections.Generic;
using Decal.Adapter;

namespace DrunkenBoxing {
    public class Fellowship {
        public static Fellowship instance = new Fellowship();
        public bool inFellowship;
        public int leaderID;
        public List<int> members;
        public bool membershipHasChanged;

        private Fellowship() {
            inFellowship = false;
            leaderID = -1;
            members = new List<int>();
            membershipHasChanged = false;
        }

        private void Disband() {
            leaderID = -1;
            members.RemoveAll(x => true);
            inFellowship = false;
            Logger.LogMessage("Fellowship disbanded.");
        }

        public void EchoFilter_ServerDispatch(object sender, NetworkMessageEventArgs e) {
            try {
                if (e.Message.Type == 0xF7B0) {
                    int type = e.Message.Value<int>("event");

                    if (type == 0x02BE) { // Create+Join
                        leaderID = e.Message.Value<int>("leader");
                        int count = e.Message.Value<int>("fellowCount");

                        for (int i = 0; i < count; i++) {
                            int fellow = e.Message.Struct("fellows").Struct(i).Struct("fellow").Value<int>("fellow");

                            if (!members.Contains(fellow)) {
                                members.Add(fellow);
                                membershipHasChanged = true;
                                // Logger.LogMessage("Fellowship create/join member added: " + fellow);
                            }
                        }

                        inFellowship = true;
                    }
                    else if (type == 0x02BF) { // Disband
                        Disband();
                    }
                    else if (type == 0x02C0) { // Member Added
                        int id = e.Message.Struct("fellow").Value<int>("fellow");

                        if (!members.Contains(id)) {
                            members.Add(id);
                            membershipHasChanged = true;
                            // Logger.LogMessage("Fellowship member added: " + id);
                        }
                    }
                    else if (type == 0x00A3) { // Member Quit
                        int id = e.Message.Value<int>("fellow");

                        if (members.Contains(id)) {
                            members.Remove(id);
                            // Logger.LogMessage("Fellowship member quit: " + id);

                            if (members.Count < 1)
                                Disband();
                        }
                    }
                    else if (type == 0x00A4) { // Member Dismissed
                        int id = e.Message.Value<int>("fellow");

                        if (members.Contains(id)) {
                            members.Remove(id);
                            // Logger.LogMessage("Fellowship member kicked: " + id);
                        }
                    }
                }
            } catch (Exception ex) { Logger.LogError("Fellowship.EchoFilter_ServerDispatch=" + e.Message.Type.ToString() + "/" + e.Message.Value<int>("event").ToString(), ex); }
        }
    }
}