namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Matches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MatchId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Map = c.String(),
                        Event = c.String(),
                        ResultT1 = c.Int(nullable: false),
                        ResultT2 = c.Int(nullable: false),
                        FirstRound1HWinTeamId = c.Int(nullable: false),
                        FirstRound2HWinTeamId = c.Int(nullable: false),
                        FirstRound1HWinCtTerr = c.String(),
                        FirstRound2HWinCtTerr = c.String(),
                        Team1Id = c.Int(nullable: false),
                        T1Player1Id = c.Int(nullable: false),
                        T1Player2Id = c.Int(nullable: false),
                        T1Player3Id = c.Int(nullable: false),
                        T1Player4Id = c.Int(nullable: false),
                        T1Player5Id = c.Int(nullable: false),
                        Team2Id = c.Int(nullable: false),
                        T2Player1Id = c.Int(nullable: false),
                        T2Player2Id = c.Int(nullable: false),
                        T2Player3Id = c.Int(nullable: false),
                        T2Player4Id = c.Int(nullable: false),
                        T2Player5Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlayerId = c.Int(nullable: false),
                        PlayerName = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Ranks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RankId = c.Int(nullable: false),
                        RankingListId = c.Int(nullable: false),
                        RankPosition = c.Int(nullable: false),
                        TeamId = c.Int(nullable: false),
                        Points = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RankingLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RankingListId = c.Int(nullable: false),
                        DateOfRank = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TeamId = c.Int(nullable: false),
                        TeamName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Teams");
            DropTable("dbo.RankingLists");
            DropTable("dbo.Ranks");
            DropTable("dbo.Players");
            DropTable("dbo.Matches");
        }
    }
}
