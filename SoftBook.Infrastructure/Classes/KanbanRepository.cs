using Microsoft.Extensions.Configuration;
using Npgsql;
using SoftBook.Domain.Data;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoftBook.Infrastructure.Classes
{
    public class KanbanRepository : BaseDAL, IKanbanRepository
    {
        public KanbanRepository(IConfiguration configuration, ITenantProvider tenantProvider)
            : base(configuration, tenantProvider)
        {
        }
        // Add this private helper at the bottom of the class
        private static string Truncate(string? value, int maxLength) =>
            string.IsNullOrEmpty(value) ? "" : value.Length <= maxLength ? value : value[..maxLength];

        public async Task EnsureSchemaAndDefaultsAsync(int companyId)
        {
            long userId = _tenantProvider?.UserId ?? 0;
            if (userId == 0)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            // Ensure checklist items table exists
            string checklistTableQuery = @"
                CREATE TABLE IF NOT EXISTS tab_kanban_checklist_items (
                    checklist_item_id BIGSERIAL PRIMARY KEY,
                    kanban_task_id BIGINT NOT NULL,
                    checklist_group VARCHAR(255),
                    item_text VARCHAR(500) NOT NULL,
                    is_completed BOOLEAN DEFAULT FALSE,
                    completed_date TIMESTAMP,
                    position_order INT DEFAULT 0,
                    created_by BIGINT,
                    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    updated_by BIGINT,
                    updated_date TIMESTAMP,
                    is_deleted BOOLEAN DEFAULT FALSE,
                    CONSTRAINT fk_checklist_task
                        FOREIGN KEY (kanban_task_id)
                        REFERENCES tab_kanban_tasks(kanban_task_id)
                        ON DELETE CASCADE
                );";
            await ExecuteNonQueryAsync(checklistTableQuery, cmd => {}, CommandType.Text);

            // Ensure a default board exists for this company and user
            string boardCountQuery = "SELECT COUNT(*) FROM tab_boards WHERE company_id = @CompanyId AND owner_user_id = @UserId AND is_deleted = false";
            long boardCount = await ExecuteScalarAsync<long>(boardCountQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@CompanyId", (long)companyId);
                cmd.Parameters.AddWithValue("@UserId", userId);
            }, CommandType.Text);

            long boardId = 0;
            if (boardCount == 0)
            {
                string insertBoardQuery = @"
                    INSERT INTO tab_boards (company_id, owner_user_id, board_name, is_default)
                    VALUES (@CompanyId, @UserId, 'Default Board', true)
                    RETURNING board_id";

                boardId = await ExecuteScalarAsync<long>(insertBoardQuery, cmd =>
                {
                    cmd.Parameters.AddWithValue("@CompanyId", (long)companyId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                }, CommandType.Text);
            }
            else
            {
                string selectBoardQuery = "SELECT board_id FROM tab_boards WHERE company_id = @CompanyId AND owner_user_id = @UserId AND is_deleted = false ORDER BY is_default DESC, board_id ASC LIMIT 1";
                boardId = await ExecuteScalarAsync<long>(selectBoardQuery, cmd =>
                {
                    cmd.Parameters.AddWithValue("@CompanyId", (long)companyId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                }, CommandType.Text);
            }

            // Ensure default columns exist for this board
            string colCountQuery = "SELECT COUNT(*) FROM tab_board_columns WHERE board_id = @BoardId AND is_deleted = false";
            long colCount = await ExecuteScalarAsync<long>(colCountQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@BoardId", boardId);
            }, CommandType.Text);

            if (colCount == 0)
            {
                var defaultColumns = new[]
                {
                    new { Name = "Today", Color = "warning", Position = 1 },
                    new { Name = "This Week", Color = "success", Position = 2 },
                    new { Name = "Later", Color = "dark", Position = 3 },
                    new { Name = "Completed", Color = "info", Position = 4 }
                };

                string insertColQuery = @"
                    INSERT INTO tab_board_columns (board_id, column_name, column_color, display_order)
                    VALUES (@BoardId, @Name, @Color, @Position)";

                foreach (var col in defaultColumns)
                {
                    await ExecuteNonQueryAsync(insertColQuery, cmd =>
                    {
                        cmd.Parameters.AddWithValue("@BoardId", boardId);
                        cmd.Parameters.AddWithValue("@Name", col.Name);
                        cmd.Parameters.AddWithValue("@Color", col.Color);
                        cmd.Parameters.AddWithValue("@Position", col.Position);
                    }, CommandType.Text);
                }
            }
        }

        private async Task ValidateBoardOwnershipAsync(long boardId, int companyId)
        {
            long userId = _tenantProvider?.UserId ?? 0;
            if (userId == 0)
                throw new UnauthorizedAccessException("User is not authenticated.");

            string query = @"
                SELECT COUNT(*)
                FROM tab_boards
                WHERE board_id = @BoardId
                AND owner_user_id = @UserId
                AND company_id = @CompanyId
                AND is_deleted = false";

            long count = await ExecuteScalarAsync<long>(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@BoardId", boardId);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CompanyId", (long)companyId);
            }, CommandType.Text);

            if (count == 0)
                throw new UnauthorizedAccessException("You do not have permission to access this board.");
        }

        public async Task<List<KanbanBoardModel>> GetBoardsAsync(int companyId)
        {
            await EnsureSchemaAndDefaultsAsync(companyId);
            long userId = _tenantProvider?.UserId ?? 0;
            if (userId == 0)
                throw new UnauthorizedAccessException("User is not authenticated.");

            string query = @"
                SELECT board_id, board_name, is_default
                FROM tab_boards
                WHERE company_id = @CompanyId AND owner_user_id = @UserId AND is_deleted = false
                ORDER BY is_default DESC, board_id ASC";

            return await ExecuteRawQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@CompanyId", (long)companyId);
                cmd.Parameters.AddWithValue("@UserId", userId);
            }, MapBoardReader);
        }

        public async Task<KanbanBoardModel> CreateBoardAsync(int companyId, string boardName)
        {
            long userId = _tenantProvider?.UserId ?? 0;
            if (userId == 0)
                throw new UnauthorizedAccessException("User is not authenticated.");

            string insertBoardQuery = @"
                INSERT INTO tab_boards (company_id, owner_user_id, board_name, is_default)
                VALUES (@CompanyId, @UserId, @BoardName, false)
                RETURNING board_id, board_name, is_default";

            var boards = await ExecuteRawQueryAsync(insertBoardQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@CompanyId", (long)companyId);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@BoardName", Truncate(boardName, 255));
            }, MapBoardReader);

            var newBoard = boards.FirstOrDefault()
                ?? throw new InvalidOperationException("Failed to create board.");

            var defaultColumns = new[]
            {
                new { Name = "Today",     Color = "warning", Position = 1 },
                new { Name = "This Week", Color = "success", Position = 2 },
                new { Name = "Later",     Color = "dark",    Position = 3 },
                new { Name = "Completed", Color = "info",    Position = 4 }
            };
            string insertColQuery = @"
                INSERT INTO tab_board_columns (board_id, column_name, column_color, display_order)
                VALUES (@BoardId, @Name, @Color, @Position)";
            foreach (var col in defaultColumns)
            {
                await ExecuteNonQueryAsync(insertColQuery, cmd =>
                {
                    cmd.Parameters.AddWithValue("@BoardId", newBoard.BoardId);
                    cmd.Parameters.AddWithValue("@Name", col.Name);
                    cmd.Parameters.AddWithValue("@Color", col.Color);
                    cmd.Parameters.AddWithValue("@Position", col.Position);
                }, CommandType.Text);
            }
            return newBoard;
        }

        public async Task<List<KanbanColumnModel>> GetColumnsAsync(int companyId, long boardId)
        {
            await ValidateBoardOwnershipAsync(boardId, companyId);

            // Fetch active columns ordered by position order
            string colQuery = "SELECT * FROM tab_board_columns WHERE board_id = @BoardId AND is_deleted = false ORDER BY display_order ASC";
            var columns = await ExecuteRawQueryAsync(colQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@BoardId", boardId);
            }, MapColumnReader);

            // Fetch active tasks with assigned user name
            string taskQuery = @"
                SELECT t.*, u.full_name as fullname,
                       (SELECT COUNT(*) FROM tab_kanban_checklist_items c WHERE c.kanban_task_id = t.kanban_task_id AND c.is_deleted = false) as total_checklist_items,
                       (SELECT COUNT(*) FROM tab_kanban_checklist_items c WHERE c.kanban_task_id = t.kanban_task_id AND c.is_completed = true AND c.is_deleted = false) as completed_checklist_items
                FROM tab_kanban_tasks t
                LEFT JOIN tab_user_master u ON t.assigned_user_id = u.user_id
                WHERE t.board_id = @BoardId AND t.is_deleted = false
                 

                ORDER BY t.is_pinned DESC, t.position_order ASC";

            var tasks = await ExecuteRawQueryAsync(taskQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@BoardId", boardId);
            }, MapTaskReader);

            // Map tasks to their respective columns
            var columnDict = columns.ToDictionary(c => c.Id);
            foreach (var task in tasks)
            {
                if (columnDict.TryGetValue(task.KanId, out var col))
                {
                    col.Cards.Add(task);
                }
            }

            // Calculate badge counts
            foreach (var col in columns)
            {
                col.Badge = col.Cards.Count;
            }

            return columns;
        }

        public async Task<bool> SaveColumnAsync(int companyId, long boardId, KanbanColumnModel column)
        {
            await ValidateBoardOwnershipAsync(boardId, companyId);

            long colId = 0;
            bool exists = false;
            bool isUpdate = long.TryParse(column.Id, out colId) && colId > 0;
            if (isUpdate)
            {
                string checkQuery = "SELECT COUNT(*) FROM tab_board_columns WHERE column_id = @ColumnId AND board_id = @BoardId";
                long count = await ExecuteScalarAsync<long>(checkQuery, cmd =>
                {
                    cmd.Parameters.AddWithValue("@ColumnId", colId);
                    cmd.Parameters.AddWithValue("@BoardId", boardId);
                }, CommandType.Text);
                exists = count > 0;
                if (!exists)
                {
                    throw new UnauthorizedAccessException("You do not have permission to access this column.");
                }
            }

            if (exists)
            {
                string updateQuery = @"
                    UPDATE tab_board_columns
                    SET column_name = @Name, column_color = @Color, display_order = @Position, updated_date = CURRENT_TIMESTAMP
                    WHERE column_id = @ColumnId AND board_id = @BoardId";

                await ExecuteNonQueryAsync(updateQuery, cmd =>
                {
                    cmd.Parameters.AddWithValue("@ColumnId", colId);
                    cmd.Parameters.AddWithValue("@BoardId", boardId);
                    cmd.Parameters.AddWithValue("@Name", column.Name);
                    cmd.Parameters.AddWithValue("@Color", column.Color ?? "primary");
                    cmd.Parameters.AddWithValue("@Position", column.Position);
                }, CommandType.Text);
            }
            else
            {
                if (column.Position == 0)
                {
                    string maxPosQuery = "SELECT COALESCE(MAX(display_order), 0) + 1 FROM tab_board_columns WHERE board_id = @BoardId";
                    column.Position = (int)await ExecuteScalarAsync<long>(maxPosQuery, cmd =>
                    {
                        cmd.Parameters.AddWithValue("@BoardId", boardId);
                    }, CommandType.Text);
                }

                string insertQuery = @"
                    INSERT INTO tab_board_columns (board_id, column_name, column_color, display_order)
                    VALUES (@BoardId, @Name, @Color, @Position)
                    RETURNING column_id";

                long newColId = await ExecuteScalarAsync<long>(insertQuery, cmd =>
                {
                    cmd.Parameters.AddWithValue("@BoardId", boardId);
                    cmd.Parameters.AddWithValue("@Name", column.Name);
                    cmd.Parameters.AddWithValue("@Color", column.Color ?? "primary");
                    cmd.Parameters.AddWithValue("@Position", column.Position);
                }, CommandType.Text);

                column.Id = newColId.ToString();
            }

            return true;
        }

        ////public async Task<bool> SaveTaskAsync(int companyId, KanbanTaskModel task)
        ////{
        ////    try
        ////    {
        ////        long boardId = await GetActiveBoardIdAsync(companyId);

        ////        long taskId = 0;
        ////        bool exists = false;
        ////        if (long.TryParse(task.Id, out taskId))
        ////        {
        ////            string checkQuery = "SELECT COUNT(*) FROM tab_kanban_tasks WHERE kanban_task_id = @TaskId AND board_id = @BoardId";
        ////            long count = await ExecuteScalarAsync<long>(checkQuery, cmd =>
        ////            {
        ////                cmd.Parameters.AddWithValue("@TaskId", taskId);
        ////                cmd.Parameters.AddWithValue("@BoardId", boardId);
        ////            }, CommandType.Text);
        ////            exists = count > 0;
        ////        }

        ////        long columnId = 0;
        ////        long.TryParse(task.KanId, out columnId);

        ////        // Serialize labels from the modern details modal, while accepting the old badge1 payload.
        ////        string labels = "";
        ////        var labelJson = task.Labels ?? task.Badge1;
        ////        if (labelJson.HasValue)
        ////        {
        ////            try
        ////            {
        ////                var badges = JsonSerializer.Deserialize<List<string>>(labelJson.Value.GetRawText());
        ////                if (badges != null)
        ////                {
        ////                    labels = string.Join(",", badges);
        ////                }
        ////            }
        ////            catch (JsonException ex)
        ////            {
        ////                // Log and continue with empty labels
        ////                Console.WriteLine($"[SaveTaskAsync] Failed to deserialize labels: {ex.Message}");
        ////            }
        ////        }

        ////        long? assignedUserId = task.AssignedUserId;
        ////        if (!assignedUserId.HasValue && task.UserImages.HasValue)
        ////        {
        ////            try
        ////            {
        ////                var users = JsonSerializer.Deserialize<List<JsonElement>>(task.UserImages.Value.GetRawText());
        ////                if (users != null && users.Count > 0)
        ////                {
        ////                    if (users[0].TryGetProperty("id", out var idProp))
        ////                    {
        ////                        if (idProp.ValueKind == JsonValueKind.Number)
        ////                        {
        ////                            assignedUserId = idProp.GetInt64();
        ////                        }
        ////                        else if (idProp.ValueKind == JsonValueKind.String && long.TryParse(idProp.GetString(), out var uid))
        ////                        {
        ////                            assignedUserId = uid;
        ////                        }
        ////                    }
        ////                }
        ////            }
        ////            catch (JsonException ex)
        ////            {
        ////                // Log and continue without assigned user
        ////                Console.WriteLine($"[SaveTaskAsync] Failed to deserialize user images: {ex.Message}");
        ////            }
        ////        }

        ////        DateTime? dueDate = null;
        ////        var dueDateText = string.IsNullOrEmpty(task.DueDate) ? task.BotId : task.DueDate;
        ////        if (!string.IsNullOrEmpty(dueDateText))
        ////        {
        ////            if (DateTime.TryParse(dueDateText, out var parsedDate))
        ////            {
        ////                dueDate = parsedDate;
        ////            }
        ////        }

        ////        DateTime? startDate = null;
        ////        if (!string.IsNullOrEmpty(task.StartDate))
        ////        {
        ////            if (DateTime.TryParse(task.StartDate, out var parsedStartDate))
        ////            {
        ////                startDate = parsedStartDate;
        ////            }
        ////        }

        ////        bool shouldUpdateCoverImage = !IsBase64Image(task.CoverImage);
        ////        object coverImageValue = string.IsNullOrWhiteSpace(task.CoverImage)
        ////            ? DBNull.Value
        ////            : Truncate(task.CoverImage, 500);

        ////        if (exists)
        ////        {
        ////            string updateQuery = @"
        ////        UPDATE tab_kanban_tasks
        ////        SET column_id = @ColumnId, title = @Title, description = @Description,
        ////            priority = @Priority, labels = @Labels, due_date = @DueDate,
        ////            start_date = @StartDate, assigned_user_id = @AssignedUserId,
        ////            is_pinned = @IsPinned,
        ////            cover_image = CASE WHEN @ShouldUpdateCoverImage THEN @CoverImage ELSE cover_image END,
        ////            position_order = @Position, updated_date = CURRENT_TIMESTAMP, module = @Module,
        ////            attachment_paths = @AttachmentPaths, attachment_names = @AttachmentNames
        ////        WHERE kanban_task_id = @TaskId AND board_id = @BoardId";

        ////            await ExecuteNonQueryAsync(updateQuery, cmd =>
        ////            {
        ////                cmd.Parameters.AddWithValue("@TaskId", taskId);
        ////                cmd.Parameters.AddWithValue("@BoardId", boardId);
        ////                cmd.Parameters.AddWithValue("@ColumnId", columnId);
        ////                cmd.Parameters.AddWithValue("@Title", Truncate(task.Title, 500));
        ////                cmd.Parameters.AddWithValue("@Description", Truncate(task.Text, 500));   //   likely culprit
        ////                cmd.Parameters.AddWithValue("@Priority", Truncate(task.Priority ?? "Medium", 50));
        ////                cmd.Parameters.AddWithValue("@Labels", Truncate(labels, 500));
        ////                cmd.Parameters.AddWithValue("@DueDate", (object)dueDate ?? DBNull.Value);
        ////                cmd.Parameters.AddWithValue("@StartDate", (object)startDate ?? DBNull.Value);
        ////                cmd.Parameters.AddWithValue("@AssignedUserId", (object)assignedUserId ?? DBNull.Value);
        ////                cmd.Parameters.AddWithValue("@IsPinned", task.IsPinned);
        ////                cmd.Parameters.AddWithValue("@ShouldUpdateCoverImage", shouldUpdateCoverImage);
        ////                cmd.Parameters.AddWithValue("@CoverImage", coverImageValue);
        ////                cmd.Parameters.AddWithValue("@Position", task.Position);
        ////                cmd.Parameters.AddWithValue("@Module", task.Module);
        ////                cmd.Parameters.AddWithValue("@AttachmentPaths", task.AttachmentPaths ?? string.Empty);
        ////                cmd.Parameters.AddWithValue("@AttachmentNames", task.AttachmentNames ?? string.Empty);
        ////            }, CommandType.Text);
        ////        }
        ////        else
        ////        {
        ////            string insertQuery = @"
        ////        WITH next_pos AS (
        ////            SELECT COALESCE(MAX(position_order), 0) + 1 AS pos
        ////            FROM tab_kanban_tasks
        ////            WHERE board_id = @BoardId AND column_id = @ColumnId AND is_deleted = false
        ////        )
        ////        INSERT INTO tab_kanban_tasks (board_id, column_id, title, description, priority, assigned_user_id, due_date, start_date, is_pinned, cover_image, labels, position_order, module, attachment_paths, attachment_names)
        ////        SELECT @BoardId, @ColumnId, @Title, @Description, @Priority, @AssignedUserId, @DueDate, @StartDate, @IsPinned, @CoverImage, @Labels,
        ////               CASE WHEN @Position = 0 THEN (SELECT pos FROM next_pos) ELSE @Position END, @Module, @AttachmentPaths, @AttachmentNames
        ////        RETURNING kanban_task_id";

        ////            long newTaskId = await ExecuteScalarAsync<long>(insertQuery, cmd =>
        ////            {
        ////                cmd.Parameters.AddWithValue("@BoardId", boardId);
        ////                cmd.Parameters.AddWithValue("@ColumnId", columnId);
        ////                cmd.Parameters.AddWithValue("@Title", Truncate(task.Title, 500));
        ////                cmd.Parameters.AddWithValue("@Description", Truncate(task.Text, 500));
        ////                cmd.Parameters.AddWithValue("@Priority", Truncate(task.Priority ?? "Medium", 50));
        ////                cmd.Parameters.AddWithValue("@AssignedUserId", (object)assignedUserId ?? DBNull.Value);
        ////                cmd.Parameters.AddWithValue("@DueDate", (object)dueDate ?? DBNull.Value);
        ////                cmd.Parameters.AddWithValue("@StartDate", (object)startDate ?? DBNull.Value);
        ////                cmd.Parameters.AddWithValue("@IsPinned", task.IsPinned);
        ////                cmd.Parameters.AddWithValue("@CoverImage", shouldUpdateCoverImage ? coverImageValue : DBNull.Value);
        ////                cmd.Parameters.AddWithValue("@Labels", Truncate(labels, 500));
        ////                cmd.Parameters.AddWithValue("@Position", task.Position);
        ////                cmd.Parameters.AddWithValue("@Module", Truncate(task.Module, 100));
        ////                cmd.Parameters.AddWithValue("@AttachmentPaths", task.AttachmentPaths ?? string.Empty);
        ////                cmd.Parameters.AddWithValue("@AttachmentNames", task.AttachmentNames ?? string.Empty);
        ////            }, CommandType.Text);

        ////            task.Id = newTaskId.ToString();
        ////            task.CardId = "#VLZ" + newTaskId;
        ////        }

        ////        return true;
        ////    }
        ////    catch (NpgsqlException ex)
        ////    {
        ////        Console.WriteLine($"[SaveTaskAsync] Database error for companyId={companyId}, taskId={task.Id}: {ex.Message}");
        ////        throw;
        ////    }
        ////    catch (TimeoutException ex)
        ////    {
        ////        Console.WriteLine($"[SaveTaskAsync] Timeout for companyId={companyId}, taskId={task.Id}: {ex.Message}");
        ////        throw;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Console.WriteLine($"[SaveTaskAsync] Unexpected error for companyId={companyId}, taskId={task.Id}: {ex.Message}");
        ////        throw;
        ////    }
        ////}
        ///
        public async Task<bool> SaveTaskAsync(int companyId, long boardId, KanbanTaskModel task)
        {
            try
            {
                await ValidateBoardOwnershipAsync(boardId, companyId);

                long taskId = 0;
                bool exists = false;
                bool isUpdate = long.TryParse(task.Id, out taskId) && taskId > 0;
                if (isUpdate)
                {
                    string checkQuery = "SELECT COUNT(*) FROM tab_kanban_tasks WHERE kanban_task_id = @TaskId AND board_id = @BoardId";
                    long count = await ExecuteScalarAsync<long>(checkQuery, cmd =>
                    {
                        cmd.Parameters.AddWithValue("@TaskId", taskId);
                        cmd.Parameters.AddWithValue("@BoardId", boardId);
                    }, CommandType.Text);
                    exists = count > 0;
                    if (!exists)
                    {
                        throw new UnauthorizedAccessException("You do not have permission to access this task.");
                    }
                }

                long columnId = 0;
                long.TryParse(task.KanId, out columnId);
                if (columnId > 0)
                {
                    string checkColQuery = "SELECT COUNT(*) FROM tab_board_columns WHERE column_id = @ColumnId AND board_id = @BoardId AND is_deleted = false";
                    long colCount = await ExecuteScalarAsync<long>(checkColQuery, cmd =>
                    {
                        cmd.Parameters.AddWithValue("@ColumnId", columnId);
                        cmd.Parameters.AddWithValue("@BoardId", boardId);
                    }, CommandType.Text);
                    if (colCount == 0)
                    {
                        throw new UnauthorizedAccessException("You do not have permission to access the specified column.");
                    }
                }

                // Serialize labels
                string labels = "";
                var labelJson = task.Labels ?? task.Badge1;
                if (labelJson.HasValue)
                {
                    try
                    {
                        var badges = JsonSerializer.Deserialize<List<string>>(labelJson.Value.GetRawText());
                        if (badges != null) labels = string.Join(",", badges);
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"[SaveTaskAsync] Failed to deserialize labels: {ex.Message}");
                    }
                }

                long? assignedUserId = task.AssignedUserId;
                if (!assignedUserId.HasValue && task.UserImages.HasValue)
                {
                    try
                    {
                        var users = JsonSerializer.Deserialize<List<JsonElement>>(task.UserImages.Value.GetRawText());
                        if (users != null && users.Count > 0)
                        {
                            if (users[0].TryGetProperty("id", out var idProp))
                            {
                                if (idProp.ValueKind == JsonValueKind.Number)
                                    assignedUserId = idProp.GetInt64();
                                else if (idProp.ValueKind == JsonValueKind.String && long.TryParse(idProp.GetString(), out var uid))
                                    assignedUserId = uid;
                            }
                        }
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"[SaveTaskAsync] Failed to deserialize user images: {ex.Message}");
                    }
                }

                DateTime? dueDate = null;
                var dueDateText = string.IsNullOrEmpty(task.DueDate) ? task.BotId : task.DueDate;
                if (!string.IsNullOrEmpty(dueDateText) && DateTime.TryParse(dueDateText, out var parsedDate))
                    dueDate = parsedDate;

                DateTime? startDate = null;
                if (!string.IsNullOrEmpty(task.StartDate) && DateTime.TryParse(task.StartDate, out var parsedStartDate))
                    startDate = parsedStartDate;

                bool shouldUpdateCoverImage = !IsBase64Image(task.CoverImage);
                object coverImageValue = string.IsNullOrWhiteSpace(task.CoverImage)
                    ? DBNull.Value
                    : Truncate(task.CoverImage, 500);

                // --- Call fn_manage_kanban_task for INSERT or UPDATE ---
                string actionType = exists ? "UPDATE" : "INSERT";

                string fnQuery = @"
            SELECT fn_manage_kanban_task(
                @ActionType, @TaskId, @BoardId, @ColumnId,
                @Title, @Description, @Priority,
                @AssignedUserId, @DueDate, @StartDate, @IsPinned
            )";

                long returnedTaskId = await ExecuteScalarAsync<long>(fnQuery, cmd =>
                {
                    cmd.Parameters.AddWithValue("@ActionType", actionType);
                    cmd.Parameters.AddWithValue("@TaskId", exists ? taskId : 0L);
                    cmd.Parameters.AddWithValue("@BoardId", boardId);
                    cmd.Parameters.AddWithValue("@ColumnId", columnId);
                    cmd.Parameters.AddWithValue("@Title", Truncate(task.Title, 500));
                    cmd.Parameters.AddWithValue("@Description", Truncate(task.Text, 500));
                    cmd.Parameters.AddWithValue("@Priority", Truncate(task.Priority ?? "Medium", 50));
                    cmd.Parameters.AddWithValue("@AssignedUserId", (object)assignedUserId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DueDate", (object)dueDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StartDate", (object)startDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPinned", task.IsPinned);
                }, CommandType.Text);

                // --- Follow-up UPDATE for fields not covered by the function ---
                string extrasQuery = exists
                    ? @"UPDATE tab_kanban_tasks
                SET labels = @Labels,
                    cover_image = CASE WHEN @ShouldUpdateCoverImage THEN @CoverImage ELSE cover_image END,
                    position_order = @Position,
                    module = @Module,
                    attachment_paths = @AttachmentPaths,
                    attachment_names = @AttachmentNames,
                    updated_date = CURRENT_TIMESTAMP
                WHERE kanban_task_id = @TaskId AND board_id = @BoardId"
                    : @"UPDATE tab_kanban_tasks
                SET labels = @Labels,
                    cover_image = CASE WHEN @ShouldUpdateCoverImage THEN @CoverImage ELSE cover_image END,
                    position_order = CASE WHEN @Position = 0 THEN position_order ELSE @Position END,
                    module = @Module,
                    attachment_paths = @AttachmentPaths,
                    attachment_names = @AttachmentNames,
                    updated_date = CURRENT_TIMESTAMP
                WHERE kanban_task_id = @TaskId AND board_id = @BoardId";

                await ExecuteNonQueryAsync(extrasQuery, cmd =>
                {
                    cmd.Parameters.AddWithValue("@TaskId", returnedTaskId);
                    cmd.Parameters.AddWithValue("@BoardId", boardId);
                    cmd.Parameters.AddWithValue("@Labels", Truncate(labels, 500));
                    cmd.Parameters.AddWithValue("@ShouldUpdateCoverImage", shouldUpdateCoverImage);
                    cmd.Parameters.AddWithValue("@CoverImage", coverImageValue);
                    cmd.Parameters.AddWithValue("@Position", task.Position);
                    cmd.Parameters.AddWithValue("@Module", Truncate(task.Module, 100));
                    cmd.Parameters.AddWithValue("@AttachmentPaths", task.AttachmentPaths ?? string.Empty);
                    cmd.Parameters.AddWithValue("@AttachmentNames", task.AttachmentNames ?? string.Empty);
                }, CommandType.Text);

                if (!exists)
                {
                    task.Id = returnedTaskId.ToString();
                    task.CardId = "#VLZ" + returnedTaskId;
                }

                return true;
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"[SaveTaskAsync] Database error for companyId={companyId}, taskId={task.Id}: {ex.Message}");
                throw;
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"[SaveTaskAsync] Timeout for companyId={companyId}, taskId={task.Id}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SaveTaskAsync] Unexpected error for companyId={companyId}, taskId={task.Id}: {ex.Message}");
                throw;
            }
        }
        public async Task<bool> DeleteColumnAsync(int companyId, long boardId, string columnId)
        {
            await ValidateBoardOwnershipAsync(boardId, companyId);
            if (long.TryParse(columnId, out var colId))
            {
                string query = "UPDATE tab_board_columns SET is_deleted = true, updated_date = CURRENT_TIMESTAMP WHERE column_id = @ColumnId AND board_id = @BoardId";
                await ExecuteNonQueryAsync(query, cmd =>
                {
                    cmd.Parameters.AddWithValue("@ColumnId", colId);
                    cmd.Parameters.AddWithValue("@BoardId", boardId);
                }, CommandType.Text);
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteTaskAsync(int companyId, long boardId, string taskId)
        {
            await ValidateBoardOwnershipAsync(boardId, companyId);
            if (long.TryParse(taskId, out var tId))
            {
                // Verify the task belongs to the active board
                string checkQuery = "SELECT COUNT(*) FROM tab_kanban_tasks WHERE kanban_task_id = @TaskId AND board_id = @BoardId AND is_deleted = false";
                long count = await ExecuteScalarAsync<long>(checkQuery, cmd =>
                {
                    cmd.Parameters.AddWithValue("@TaskId", tId);
                    cmd.Parameters.AddWithValue("@BoardId", boardId);
                }, CommandType.Text);
                if (count == 0)
                {
                    throw new UnauthorizedAccessException("You do not have permission to delete this task.");
                }

                string fnQuery = "SELECT fn_manage_kanban_task(@ActionType, @TaskId)";
                await ExecuteScalarAsync<long>(fnQuery, cmd =>
                {
                    cmd.Parameters.AddWithValue("@ActionType", "DELETE");
                    cmd.Parameters.AddWithValue("@TaskId", tId);
                }, CommandType.Text);
                return true;
            }
            return false;
        }
        //public async Task<bool> DeleteTaskAsync(int companyId, string taskId)
        //{
        //    long boardId = await GetActiveBoardIdAsync(companyId);
        //    if (long.TryParse(taskId, out var tId))
        //    {
        //        string query = "UPDATE tab_kanban_tasks SET is_deleted = true, updated_date = CURRENT_TIMESTAMP WHERE kanban_task_id = @TaskId AND board_id = @BoardId";
        //        await ExecuteNonQueryAsync(query, cmd =>
        //        {
        //            cmd.Parameters.AddWithValue("@TaskId", tId);
        //            cmd.Parameters.AddWithValue("@BoardId", boardId);
        //        }, CommandType.Text);
        //        return true;
        //    }
        //    return false;
        //}

        private static KanbanBoardModel MapBoardReader(NpgsqlDataReader reader) => new KanbanBoardModel
        {
            BoardId = Convert.ToInt64(reader["board_id"]),
            BoardName = reader["board_name"]?.ToString() ?? "",
            IsDefault = reader["is_default"] != DBNull.Value && Convert.ToBoolean(reader["is_default"])
        };

        private static KanbanColumnModel MapColumnReader(NpgsqlDataReader reader) => new KanbanColumnModel
        {
            Id = reader["column_id"].ToString() ?? "",
            Name = reader["column_name"]?.ToString() ?? "",
            Color = reader["column_color"]?.ToString() ?? "primary",
            Position = Convert.ToInt32(reader["display_order"]),
            Badge = 0,
            Cards = new List<KanbanTaskModel>()
        };

        private static KanbanTaskModel MapTaskReader(NpgsqlDataReader reader)
        {
            var taskId = reader["kanban_task_id"].ToString() ?? "";
            var labelsRaw = reader["labels"]?.ToString() ?? "";
            
            // Format labels as JSON array of strings
            JsonElement? badge1Val = null;
            if (!string.IsNullOrEmpty(labelsRaw))
            {
                var labelsList = labelsRaw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                           .Select(l => l.Trim())
                                           .ToList();
                badge1Val = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(labelsList));
            }
            else
            {
                badge1Val = JsonSerializer.Deserialize<JsonElement>("[]");
            }

            // Map assigned user image profile card
            var assignedUserId = reader["assigned_user_id"] != DBNull.Value ? Convert.ToInt64(reader["assigned_user_id"]) : 0;
            var fullname = reader["fullname"] != DBNull.Value ? reader["fullname"].ToString() : "";
            JsonElement? userImagesVal = null;

            if (assignedUserId > 0)
            {
                var members = new[]
                {
                    new
                    {
                        id = assignedUserId,
                        name = string.IsNullOrEmpty(fullname) ? "Member" : fullname,
                        img = $"/avatar-{(assignedUserId % 10) + 1}.jpg"
                    }
                };
                userImagesVal = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(members));
            }
            else
            {
                userImagesVal = JsonSerializer.Deserialize<JsonElement>("[]");
            }

            // Format date to: dd MMM, yyyy
            var dueDate = reader["due_date"] != DBNull.Value ? Convert.ToDateTime(reader["due_date"]) : (DateTime?)null;
            var botIdStr = dueDate.HasValue ? dueDate.Value.ToString("dd MMM, yyyy") : "";

            var totalChecklistCount = reader["total_checklist_items"] != DBNull.Value ? Convert.ToInt32(reader["total_checklist_items"]) : 0;
            var completedChecklistCount = reader["completed_checklist_items"] != DBNull.Value ? Convert.ToInt32(reader["completed_checklist_items"]) : 0;

            return new KanbanTaskModel
            {
                Id = taskId,
                KanId = reader["column_id"].ToString() ?? "",
                CardId = "#VLZ" + taskId,
                Title = reader["title"]?.ToString() ?? "",
                Text = reader["description"]?.ToString() ?? "",
                Priority = reader["priority"]?.ToString() ?? "Medium",
                Module = reader["module"]?.ToString() ?? "",
                Badge1 = badge1Val,
                Labels = badge1Val,
                BotId = botIdStr,
                DueDate = dueDate.HasValue ? dueDate.Value.ToString("yyyy-MM-dd") : "",
                StartDate = reader["start_date"] != DBNull.Value ? Convert.ToDateTime(reader["start_date"]).ToString("yyyy-MM-dd") : "",
                IsPinned = reader["is_pinned"] != DBNull.Value && Convert.ToBoolean(reader["is_pinned"]),
                CoverImage = reader["cover_image"]?.ToString() ?? "",
                AssignedUserId = assignedUserId > 0 ? assignedUserId : null,
                UserImages = userImagesVal,
                Eye = 3,
                Que = 2,
                Clip = 1,
                Position = Convert.ToInt32(reader["position_order"]),
                AttachmentPaths = reader["attachment_paths"] != DBNull.Value ? reader["attachment_paths"].ToString() ?? "" : "",
                AttachmentNames = reader["attachment_names"] != DBNull.Value ? reader["attachment_names"].ToString() ?? "" : "",
                TotalChecklistCount = totalChecklistCount,
                CompletedChecklistCount = completedChecklistCount
            };
        }

        public async Task<List<KanbanChecklistItemModel>> GetChecklistItemsAsync(long taskId)
        {
            long userId = _tenantProvider?.UserId ?? 0;
            int companyId = _tenantProvider?.CompanyId ?? 0;

            // Verify task belongs to a board owned by this user
            string taskAccessQuery = @"
                SELECT COUNT(*) 
                FROM tab_kanban_tasks t
                JOIN tab_boards b ON t.board_id = b.board_id
                WHERE t.kanban_task_id = @TaskId 
                  AND b.company_id = @CompanyId 
                  AND b.owner_user_id = @UserId 
                  AND t.is_deleted = false 
                  AND b.is_deleted = false";

            long accessCount = await ExecuteScalarAsync<long>(taskAccessQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@TaskId", taskId);
                cmd.Parameters.AddWithValue("@CompanyId", (long)companyId);
                cmd.Parameters.AddWithValue("@UserId", userId);
            }, CommandType.Text);

            if (accessCount == 0)
            {
                throw new UnauthorizedAccessException("You do not have permission to access checklist items for this task.");
            }

            string query = "SELECT * FROM tab_kanban_checklist_items WHERE kanban_task_id = @TaskId AND is_deleted = false ORDER BY position_order ASC, checklist_item_id ASC";
            return await ExecuteRawQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@TaskId", taskId);
            }, MapChecklistItemReader);
        }

        public async Task<KanbanChecklistItemModel> SaveChecklistItemAsync(KanbanChecklistItemModel item)
        {
            long userId = _tenantProvider?.UserId ?? 0;
            int companyId = _tenantProvider?.CompanyId ?? 0;

            // Verify task belongs to a board owned by this user
            string taskAccessQuery = @"
                SELECT COUNT(*) 
                FROM tab_kanban_tasks t
                JOIN tab_boards b ON t.board_id = b.board_id
                WHERE t.kanban_task_id = @TaskId 
                  AND b.company_id = @CompanyId 
                  AND b.owner_user_id = @UserId 
                  AND t.is_deleted = false 
                  AND b.is_deleted = false";

            long accessCount = await ExecuteScalarAsync<long>(taskAccessQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@TaskId", item.KanbanTaskId);
                cmd.Parameters.AddWithValue("@CompanyId", (long)companyId);
                cmd.Parameters.AddWithValue("@UserId", userId);
            }, CommandType.Text);

            if (accessCount == 0)
            {
                throw new UnauthorizedAccessException("You do not have permission to access checklist items for this task.");
            }

            bool exists = false;
            if (item.ChecklistItemId > 0)
            {
                string checkQuery = "SELECT COUNT(*) FROM tab_kanban_checklist_items WHERE checklist_item_id = @ItemId AND kanban_task_id = @TaskId";
                long count = await ExecuteScalarAsync<long>(checkQuery, cmd =>
                {
                    cmd.Parameters.AddWithValue("@ItemId", item.ChecklistItemId);
                    cmd.Parameters.AddWithValue("@TaskId", item.KanbanTaskId);
                }, CommandType.Text);
                exists = count > 0;
                if (!exists)
                {
                    throw new UnauthorizedAccessException("Checklist item does not exist or does not belong to the specified task.");
                }
            }

            if (exists)
            {
                string updateQuery = @"
                    UPDATE tab_kanban_checklist_items
                    SET checklist_group = @ChecklistGroup,
                        item_text = @ItemText,
                        is_completed = @IsCompleted,
                        completed_date = @CompletedDate,
                        position_order = @PositionOrder,
                        updated_by = @UpdatedBy,
                        updated_date = CURRENT_TIMESTAMP
                    WHERE checklist_item_id = @ChecklistItemId
                    RETURNING *";

                var completedDateValue = item.IsCompleted ? (object)(item.CompletedDate ?? DateTime.UtcNow) : DBNull.Value;

                var updatedList = await ExecuteRawQueryAsync(updateQuery, cmd =>
                {
                    cmd.Parameters.AddWithValue("@ChecklistItemId", item.ChecklistItemId);
                    cmd.Parameters.AddWithValue("@ChecklistGroup", Truncate(item.ChecklistGroup, 255));
                    cmd.Parameters.AddWithValue("@ItemText", Truncate(item.ItemText, 500));
                    cmd.Parameters.AddWithValue("@IsCompleted", item.IsCompleted);
                    cmd.Parameters.AddWithValue("@CompletedDate", completedDateValue);
                    cmd.Parameters.AddWithValue("@PositionOrder", item.PositionOrder);
                    cmd.Parameters.AddWithValue("@UpdatedBy", (object)item.UpdatedBy ?? DBNull.Value);
                }, MapChecklistItemReader);

                return updatedList.FirstOrDefault() ?? item;
            }
            else
            {
                string insertQuery = @"
                    INSERT INTO tab_kanban_checklist_items (
                        kanban_task_id, checklist_group, item_text, is_completed, completed_date, position_order, created_by, created_date
                    )
                    VALUES (
                        @KanbanTaskId, @ChecklistGroup, @ItemText, @IsCompleted, @CompletedDate, @PositionOrder, @CreatedBy, CURRENT_TIMESTAMP
                    )
                    RETURNING *";

                var completedDateValue = item.IsCompleted ? (object)(item.CompletedDate ?? DateTime.UtcNow) : DBNull.Value;

                var insertedList = await ExecuteRawQueryAsync(insertQuery, cmd =>
                {
                    cmd.Parameters.AddWithValue("@KanbanTaskId", item.KanbanTaskId);
                    cmd.Parameters.AddWithValue("@ChecklistGroup", Truncate(item.ChecklistGroup, 255));
                    cmd.Parameters.AddWithValue("@ItemText", Truncate(item.ItemText, 500));
                    cmd.Parameters.AddWithValue("@IsCompleted", item.IsCompleted);
                    cmd.Parameters.AddWithValue("@CompletedDate", completedDateValue);
                    cmd.Parameters.AddWithValue("@PositionOrder", item.PositionOrder);
                    cmd.Parameters.AddWithValue("@CreatedBy", (object)item.CreatedBy ?? DBNull.Value);
                }, MapChecklistItemReader);

                return insertedList.FirstOrDefault() ?? item;
            }
        }

        public async Task<bool> DeleteChecklistItemAsync(long itemId)
        {
            long userId = _tenantProvider?.UserId ?? 0;
            int companyId = _tenantProvider?.CompanyId ?? 0;

            // Verify checklist item belongs to task on a board owned by this user
            string checklistAccessQuery = @"
                SELECT COUNT(*) 
                FROM tab_kanban_checklist_items c
                JOIN tab_kanban_tasks t ON c.kanban_task_id = t.kanban_task_id
                JOIN tab_boards b ON t.board_id = b.board_id
                WHERE c.checklist_item_id = @ItemId 
                  AND b.company_id = @CompanyId 
                  AND b.owner_user_id = @UserId 
                  AND c.is_deleted = false 
                  AND t.is_deleted = false 
                  AND b.is_deleted = false";

            long accessCount = await ExecuteScalarAsync<long>(checklistAccessQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@ItemId", itemId);
                cmd.Parameters.AddWithValue("@CompanyId", (long)companyId);
                cmd.Parameters.AddWithValue("@UserId", userId);
            }, CommandType.Text);

            if (accessCount == 0)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this checklist item.");
            }

            string query = "UPDATE tab_kanban_checklist_items SET is_deleted = true, updated_date = CURRENT_TIMESTAMP WHERE checklist_item_id = @ItemId";
            await ExecuteNonQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@ItemId", itemId);
            }, CommandType.Text);
            return true;
        }

        public async Task<bool> DeleteChecklistGroupAsync(long taskId, string groupName)
        {
            long userId = _tenantProvider?.UserId ?? 0;
            int companyId = _tenantProvider?.CompanyId ?? 0;

            // Verify task belongs to a board owned by this user
            string taskAccessQuery = @"
                SELECT COUNT(*) 
                FROM tab_kanban_tasks t
                JOIN tab_boards b ON t.board_id = b.board_id
                WHERE t.kanban_task_id = @TaskId 
                  AND b.company_id = @CompanyId 
                  AND b.owner_user_id = @UserId 
                  AND t.is_deleted = false 
                  AND b.is_deleted = false";

            long accessCount = await ExecuteScalarAsync<long>(taskAccessQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@TaskId", taskId);
                cmd.Parameters.AddWithValue("@CompanyId", (long)companyId);
                cmd.Parameters.AddWithValue("@UserId", userId);
            }, CommandType.Text);

            if (accessCount == 0)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete checklist items for this task.");
            }

            string query = "UPDATE tab_kanban_checklist_items SET is_deleted = true, updated_date = CURRENT_TIMESTAMP WHERE kanban_task_id = @TaskId AND checklist_group = @ChecklistGroup";
            await ExecuteNonQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@TaskId", taskId);
                cmd.Parameters.AddWithValue("@ChecklistGroup", groupName);
            }, CommandType.Text);
            return true;
        }

        private static KanbanChecklistItemModel MapChecklistItemReader(NpgsqlDataReader reader) => new KanbanChecklistItemModel
        {
            ChecklistItemId = Convert.ToInt64(reader["checklist_item_id"]),
            KanbanTaskId = Convert.ToInt64(reader["kanban_task_id"]),
            ChecklistGroup = reader["checklist_group"]?.ToString() ?? string.Empty,
            ItemText = reader["item_text"]?.ToString() ?? string.Empty,
            IsCompleted = Convert.ToBoolean(reader["is_completed"]),
            CompletedDate = reader["completed_date"] != DBNull.Value ? Convert.ToDateTime(reader["completed_date"]) : (DateTime?)null,
            PositionOrder = Convert.ToInt32(reader["position_order"]),
            CreatedBy = reader["created_by"] != DBNull.Value ? Convert.ToInt64(reader["created_by"]) : (long?)null,
            CreatedDate = Convert.ToDateTime(reader["created_date"]),
            UpdatedBy = reader["updated_by"] != DBNull.Value ? Convert.ToInt64(reader["updated_by"]) : (long?)null,
            UpdatedDate = reader["updated_date"] != DBNull.Value ? Convert.ToDateTime(reader["updated_date"]) : (DateTime?)null,
            IsDeleted = Convert.ToBoolean(reader["is_deleted"])
        };

        private static bool IsBase64Image(string value)
        {
            return !string.IsNullOrWhiteSpace(value)
                && value.TrimStart().StartsWith("data:image/", StringComparison.OrdinalIgnoreCase);
        }
    }
}
