﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Enums;

namespace Streetcode.DAL.Entities.Comment
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string UserFullName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? DateModified { get; set; }
        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = null!;
        [Required]
        public int StreetcodeId { get; set; }
        public StreetcodeContent Streetcode { get; set; } = null!;
        public int? ParentId { get; set; }
        public Comment? Parent { get; set; }
        public List<Comment>? Children { get; set; }
        public CommentStatus Status { get; set; }
    }
}
