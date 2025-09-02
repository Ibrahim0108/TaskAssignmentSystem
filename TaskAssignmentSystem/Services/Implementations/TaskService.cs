using TaskAssignmentSystem.Data;
using TaskAssignmentSystem.Models.Task;
using TaskAssignmentSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace TaskAssignmentSystem.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _db;

        public TaskService(ApplicationDbContext db)
        {
            _db = db;
        }

        public WorkspaceTask AssignTask(int workspaceId, string title, string description)
        {
            var task = new WorkspaceTask
            {
                WorkspaceId = workspaceId,
                Title = title,
                Description = description
            };

            _db.WorkspaceTasks.Add(task);
            _db.SaveChanges();

            return task;
        }

        public void UpdateProgress(int taskId, int studentId, int progress)
        {
            var tp = _db.TaskProgresses
                        .FirstOrDefault(p => p.TaskId == taskId && p.StudentId == studentId);

            if (tp == null)
            {
                tp = new TaskProgress
                {
                    TaskId = taskId,
                    StudentId = studentId,
                    ProgressPercent = progress,
                    UpdatedAt = DateTime.Now
                };
                _db.TaskProgresses.Add(tp);
            }
            else
            {
                tp.ProgressPercent = progress;
                tp.UpdatedAt = DateTime.Now;
                _db.TaskProgresses.Update(tp);
            }

            _db.SaveChanges();
        }

        public WorkspaceTask? GetTaskById(int id)
        {
            return _db.WorkspaceTasks.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<WorkspaceTask> GetTasksByWorkspace(int workspaceId)
        {
            return _db.WorkspaceTasks
                      .Include(t => t.ProgressUpdates) // load progress for each task
                      .Where(t => t.WorkspaceId == workspaceId)
                      .ToList();
        }
    }
}
