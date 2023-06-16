namespace DungeonBoard.Models
{
    public enum SkillType
    {
        Fixed = 0, 
        Physic = 1, 
        Heal = 2
    }
    public enum ClassType
    {
        NoneClass = 0,
        WarriorClass = 1, 
        WizardClass = 2, 
        ArcherClass = 3, 
    }

    public enum GameResult
    {
        GameWin = 0, 
        GameDefeat = 1,
        GameProceeding = 2
    }
}
