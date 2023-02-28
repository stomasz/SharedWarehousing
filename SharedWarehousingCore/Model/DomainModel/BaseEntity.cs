// using System;
// using System.ComponentModel.DataAnnotations.Schema;
// using DataAccess.Model.IdentityModel;
//
// namespace DataAccess.Model.DomainModel
// {
//     public abstract class BaseEntity
//     {
//         public int Id { get; set; }
//         public DateTime CreatedDateTime { get; set; }
//
//         [ForeignKey("CreatedByUserId")]
//         public int CreatedByUserId { get; set; }
//
//         public AppUser CreatedByUser { get; set; }
//         public DateTime? UpdatedDateTime { get; set; }
//
//         [ForeignKey("UpdatedByUserId")]
//         public int UpdatedByUserId { get; set; }
//         public AppUser UpdatedByUser { get; set; }
//     }
// }