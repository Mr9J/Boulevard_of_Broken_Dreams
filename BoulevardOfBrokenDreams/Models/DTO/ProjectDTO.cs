﻿namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class ProjectDTO
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public decimal ProjectGoal { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int MemberId { get; set; }
        public int? GroupId { get; set; }
        public string? Thumbnail { get; set; }
        public int StatusId { get; set; }
        public decimal ProjectAmount { get; set; }
        public List<ProductDTO>? Products { get; set; }
    }
}