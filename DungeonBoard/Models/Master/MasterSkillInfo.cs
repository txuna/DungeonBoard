namespace DungeonBoard.Models.Master
{
    public class MasterSkillInfo
    {
        public int SkillId { get; set; }
        public string Name { get; set; }
        public SkillType Type { get; set; }
        public int BaseValue { get; set; }
        public int Attack { get; set; }
        public int Magic { get; set; }
        public int Defence { get; set; }
        public int Mp { get; set; }
        public string Comment { get; set; }
    }
}
