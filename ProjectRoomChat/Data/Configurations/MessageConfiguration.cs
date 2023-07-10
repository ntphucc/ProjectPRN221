using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectRoomChat.Models;

namespace ProjectRoomChat.Data.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");

            builder.Property(x => x.Content).IsRequired().HasMaxLength(500);

            builder.HasOne(x => x.ToRoom)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.ToRoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
