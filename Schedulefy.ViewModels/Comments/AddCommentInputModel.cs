using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Schedulefy.OCommon.ValidationConstants.Comment;

namespace Schedulefy.ViewModels.Comments
{
    public class AddCommentInputModel
    {
        [Required]
        [StringLength(CommentContentMaxLength, MinimumLength = CommentContentMinLength)]
        public string Content { get; set; } = null!;
        public int EventId { get; set; }
    }
}
