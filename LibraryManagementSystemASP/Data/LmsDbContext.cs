using System;
using System.Collections.Generic;
using LibraryManagementSystemASP.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemASP.Data;

public partial class LmsDbContext : DbContext
{
    public LmsDbContext()
    {
    }

    public LmsDbContext(DbContextOptions<LmsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Borrowing> Borrowings { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DatabaseConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Books__490D1AE124FEDA50");

            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.Author)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("author");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("category");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Isbn)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("isbn");
            entity.Property(e => e.PublishedYear).HasColumnName("published_year");
            entity.Property(e => e.Publisher)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("publisher");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Available")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Borrowing>(entity =>
        {
            entity.HasKey(e => e.BorrowId).HasName("PK__Borrowin__262B57A05A911D3B");

            entity.ToTable("Borrowing", tb =>
                {
                    tb.HasTrigger("trg_SetBorrowingReturnDate");
                    tb.HasTrigger("trg_UpdateActualReturnDate");
                    tb.HasTrigger("trg_UpdateBookQuantityOnBorrowingReturn");
                });

            entity.Property(e => e.BorrowId).HasColumnName("borrow_id");
            entity.Property(e => e.ActualReturnDate)
                .HasColumnType("datetime")
                .HasColumnName("actual_return_date");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.BorrowDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("borrow_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.SupposedReturnDate)
                .HasColumnType("datetime")
                .HasColumnName("supposed_return_date");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Book).WithMany(p => p.Borrowings)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Borrowing__book___49C3F6B7");

            entity.HasOne(d => d.User).WithMany(p => p.Borrowings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Borrowing__user___48CFD27E");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__31384C29ACB1907C");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("trg_DecrementBookQuantityOnReservation");
                    tb.HasTrigger("trg_ReservationCollectedToBorrowing");
                    tb.HasTrigger("trg_UpdateBookQuantityOnReservationStatusChange");
                    tb.HasTrigger("trg_UpdateCollectedAt");
                });

            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.CollectedOn)
                .HasColumnType("datetime")
                .HasColumnName("collected_on");
            entity.Property(e => e.CollectionDeadline)
                .HasColumnType("datetime")
                .HasColumnName("collection_deadline");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Book).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservati__book___440B1D61");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservati__user___4316F928");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F55F37ADB");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Reader")
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
