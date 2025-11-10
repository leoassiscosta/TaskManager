using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure;

public class DbSeeder
{
    private readonly ApplicationDbContext _context;

    public DbSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Aplicar migrations pendentes
        await _context.Database.MigrateAsync();

        // Verificar se já existem dados
        if (await _context.Users.AnyAsync())
        {
            return; // Banco já tem dados
        }

        // Criar usuários de exemplo
        var user1 = new User
        {
            Name = "João Silva",
            Email = "joao.silva@example.com",
            Role = UserRole.User
        };

        var manager = new User
        {
            Name = "Maria Santos",
            Email = "maria.santos@example.com",
            Role = UserRole.Manager
        };

        var user2 = new User
        {
            Name = "Carlos Oliveira",
            Email = "carlos.oliveira@example.com",
            Role = UserRole.User
        };

        await _context.Users.AddRangeAsync(user1, manager, user2);
        await _context.SaveChangesAsync();

        // Criar projeto de exemplo para user1
        var project1 = new Project
        {
            Name = "Website Redesign",
            Description = "Redesign completo do website da empresa",
            UserId = user1.Id
        };

        var project2 = new Project
        {
            Name = "Mobile App",
            Description = "Desenvolvimento de aplicativo mobile",
            UserId = user1.Id
        };

        var project3 = new Project
        {
            Name = "API Integration",
            Description = "Integração com APIs de terceiros",
            UserId = user2.Id
        };

        await _context.Projects.AddRangeAsync(project1, project2, project3);
        await _context.SaveChangesAsync();

        // Criar tarefas de exemplo
        var task1 = new ProjectTask
        {
            Title = "Criar wireframes",
            Description = "Criar wireframes das principais páginas",
            DueDate = DateTime.UtcNow.AddDays(7),
            Status = TasksStatus.InProgress,
            Priority = TaskPriority.High,
            ProjectId = project1.Id
        };

        var task2 = new ProjectTask
        {
            Title = "Definir paleta de cores",
            Description = "Escolher cores do novo design",
            DueDate = DateTime.UtcNow.AddDays(5),
            Status = TasksStatus.Completed,
            Priority = TaskPriority.Medium,
            ProjectId = project1.Id
        };

        var task3 = new ProjectTask
        {
            Title = "Implementar tela de login",
            Description = "Desenvolver interface e lógica da tela de login",
            DueDate = DateTime.UtcNow.AddDays(10),
            Status = TasksStatus.Pending,
            Priority = TaskPriority.High,
            ProjectId = project2.Id
        };

        await _context.Tasks.AddRangeAsync(task1, task2, task3);
        await _context.SaveChangesAsync();

        // Criar comentário de exemplo
        var comment1 = new TaskComment
        {
            TaskId = task1.Id,
            UserId = manager.Id,
            Content = "Ótimo progresso! Continue assim."
        };

        await _context.TaskComments.AddAsync(comment1);

        // Criar histórico de exemplo
        var history1 = new TaskHistory
        {
            TaskId = task2.Id,
            UserId = user1.Id,
            ChangeDescription = "Status updated",
            PreviousValue = "InProgress",
            NewValue = "Completed"
        };

        await _context.TaskHistories.AddAsync(history1);
        await _context.SaveChangesAsync();
    }
}
