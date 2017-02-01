﻿using CsScraperMVC.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CsScraperMVC.Data
{
    public class dataWorker
    {
        private DatabaseContext _db;

        public dataWorker()
        {
            _db = new DatabaseContext();
        }


        #region MATCHES

        public List<MatchesViewModel> GetMatches(int TeamId)
        {
            var query = _db.Match.Where(k => k.Team1Id == TeamId || k.Team2Id == TeamId).ToList();

            var matches = (from p in query
                           select new MatchesViewModel
                           {
                               Id = p.Id,
                               MatchId = p.MatchId,
                               Date = p.Date,
                               Map = p.Map,
                               Event = p.Event,
                               ResultT1 = p.ResultT1,
                               ResultT2 = p.ResultT2,
                               Team1Name = _db.Team.FirstOrDefault(k => k.TeamId == p.Team1Id).TeamName,
                               Team2Name = _db.Team.FirstOrDefault(k => k.TeamId == p.Team2Id).TeamName
                           }).ToList();

            return matches;
        }

        public Match GetMatchDetails(int? id)
        {
            var result = _db.Match.Find(id);

            return result;
        }

        #endregion

        #region TEAMS

        public List<Team> GetTeamsList()
        {
            var result = _db.Team.ToList();

            return result;
        }

        public Team GetTeamDetails(int? id)
        {
            var result = _db.Team.Find(id);

            return result;
        }

        public string GetTeamName(int id)
        {
            var result = "";

            result = _db.Team.SingleOrDefault(p => p.TeamId == id).TeamName;

            return result ?? "";
        }

        #endregion

        #region COMPARE

        public TeamStatisticPeriodModel GetTeamPeriodStatistics(int TeamId)
        {
            var result = new TeamStatisticPeriodModel();
            result.TeamId = TeamId;
            result.TeamName = GetTeamName(TeamId);
            result.TeamStatistics.Add(GetTeamPeriodStatistics(TeamId, PeriodEnum.ThreeMonths));
            result.TeamStatistics.Add(GetTeamPeriodStatistics(TeamId, PeriodEnum.Year));

            return result;
        }

        public TeamStatisticModel GetTeamPeriodStatistics(int TeamId, PeriodEnum period)
        {
            var result = new TeamStatisticModel();
            var dTo = DateTime.Now;
            var dFrom = new DateTime();

            switch (period)
            {
                case PeriodEnum.ThreeMonths:
                    dFrom = dTo.AddMonths(-3);
                    result.Period = "3 Months";
                    break;
                case PeriodEnum.SixMonths:
                    dFrom = dTo.AddMonths(-6);
                    result.Period = "6 Months";
                    break;
                case PeriodEnum.Year:
                    dFrom = dTo.AddYears(-1);
                    result.Period = "Year";
                    break;
            }

            result.Maps = GetMapStatistics(TeamId, dFrom, dTo);
            
            return result;
        }
           
        public List<MapStatisticModel> GetMapStatistics(int TeamId, DateTime dFrom, DateTime dTo)
        {
            var result = new List<MapStatisticModel>();

            var matches = _db.Match
                .Where(p => (p.Team1Id == TeamId || p.Team2Id == TeamId)
                && (p.Date >= dFrom.Date && p.Date <= dTo.Date))
                .GroupBy(k => k.Map).ToList();
            
            result = matches.Select(n => new MapStatisticModel
            {
                Map = n.Key,
                TotalWins = n.Count(p => (p.Team1Id == TeamId) ? p.ResultT1 > p.ResultT2 : p.ResultT2 > p.ResultT1),
                TotalLosses = n.Count(p => (p.Team1Id == TeamId) ? p.ResultT2 > p.ResultT1 : p.ResultT1 > p.ResultT2),
                WinPercent = Math.Round(n.Count(p => (p.Team1Id == TeamId) ? p.ResultT1 > p.ResultT2 : p.ResultT2 > p.ResultT1) / (double)n.Count() * 100, 1),
                // *** Get Average Win rounds ***
                AverageWinRounds = Math.Round(n.Sum(p => (p.Team1Id == TeamId) ? p.ResultT1 : p.ResultT2) / (double)n.Count(), 1),
                // *** Get Average Loss rounds ***
                AverageLossRounds = Math.Round(n.Sum(p => (p.Team1Id == TeamId) ? p.ResultT2 : p.ResultT1) / (double)n.Count(), 1),
                // *** Get Average Win rounds when team has won ***
                AverageWinRoundsWhenWin = Math.Round(n.Where(p => (p.Team1Id == TeamId) ? p.ResultT1 > p.ResultT2 : p.ResultT2 > p.ResultT1) // Where team won
                    .Sum(p => (p.Team1Id == TeamId) ? p.ResultT1 : p.ResultT2) / // Sum team rounds
                    (double)n.Count(p => (p.Team1Id == TeamId) ? p.ResultT1 > p.ResultT2 : p.ResultT2 > p.ResultT1), 1), // Divided total games we won
                // *** Get Average Loss rounds when team has won ***
                AverageLossRoundsWhenWin = Math.Round(n.Where(p => (p.Team1Id == TeamId) ? p.ResultT1 > p.ResultT2 : p.ResultT2 > p.ResultT1) // Where team won
                    .Sum(p => (p.Team1Id == TeamId) ? p.ResultT2 : p.ResultT1) / // Sum opponents rounds
                    (double)n.Count(p => (p.Team1Id == TeamId) ? p.ResultT1 > p.ResultT2 : p.ResultT2 > p.ResultT1), 1), // Divided total games we won
                // *** Get Average Win rounds when team has lost ***
                AverageWinRoundsWhenLoss = Math.Round(n.Where(p => (p.Team1Id == TeamId) ? p.ResultT1 < p.ResultT2 : p.ResultT2 < p.ResultT1) // Where team lost
                    .Sum(p => (p.Team1Id == TeamId) ? p.ResultT1 : p.ResultT2) / // Sum team rounds
                    (double)n.Count(p => (p.Team1Id == TeamId) ? p.ResultT1 < p.ResultT2 : p.ResultT2 < p.ResultT1), 1), // Divided total games we lost
                // *** Get Average Loss rounds when team has lost ***
                AverageLossRoundsWhenLoss = Math.Round(n.Where(p => (p.Team1Id == TeamId) ? p.ResultT1 < p.ResultT2 : p.ResultT2 < p.ResultT1) // Where team lost
                    .Sum(p => (p.Team1Id == TeamId) ? p.ResultT2 : p.ResultT1) / // Sum opponents rounds
                    (double)n.Count(p => (p.Team1Id == TeamId) ? p.ResultT1 < p.ResultT2 : p.ResultT2 < p.ResultT1), 1) // Divided total games we lost
            }).OrderByDescending(n => n.WinPercent).ToList();

            return result;
        }

        #endregion
    }

    public enum PeriodEnum
    {
        ThreeMonths = 1,
        SixMonths = 2,
        Year = 3
    }
}