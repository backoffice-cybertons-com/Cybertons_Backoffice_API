using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftBook.Application.Services.Classes
{
    public class RecurrenceService : IRecurrenceService
    {
        private const int MAX_OCCURRENCES = 500; // Safety limit to prevent infinite loops

        public List<TaskModel> GenerateOccurrences(TaskModel parentTask)
        {
            if (!parentTask.IsRecurring || string.IsNullOrEmpty(parentTask.RecurrenceType) || parentTask.RecurrenceParentId != null)
                return new List<TaskModel>();

            // RecurringGroupId is now assigned in TaskService before saving the parent

            return parentTask.RecurrenceType.ToUpper() switch
            {
                "DAILY" => GenerateDailyOccurrences(parentTask),
                "WEEKLY" => GenerateWeeklyOccurrences(parentTask),
                "MONTHLY" => GenerateMonthlyOccurrences(parentTask),
                "YEARLY" => GenerateYearlyOccurrences(parentTask),
                _ => new List<TaskModel>()
            };
        }

        private List<TaskModel> GenerateDailyOccurrences(TaskModel parent)
        {
            var occurrences = new List<TaskModel>();
            int interval = parent.RecurrenceInterval > 0 ? parent.RecurrenceInterval : 1;
            DateTime? currentAssignedDate = parent.AssignedDate;

            if (!currentAssignedDate.HasValue) return occurrences;

            int count = 0;
            while (ShouldContinue(parent, currentAssignedDate, count + 1))
            {
                currentAssignedDate = currentAssignedDate.Value.AddDays(interval);
                
                // Double check stop condition after increment
                if (!ShouldContinue(parent, currentAssignedDate, count + 1)) break;

                var child = CreateChildTask(parent, currentAssignedDate.Value);
                occurrences.Add(child);
                count++;
            }

            return occurrences;
        }

        private List<TaskModel> GenerateWeeklyOccurrences(TaskModel parent)
        {
            var occurrences = new List<TaskModel>();
            int interval = parent.RecurrenceInterval > 0 ? parent.RecurrenceInterval : 1;
            DateTime? startDate = parent.AssignedDate;

            if (!startDate.HasValue) return occurrences;

            var selectedDays = ParseWeekdays(parent.RecurrenceDays);
            if (!selectedDays.Any()) return occurrences;

            int count = 0;
            DateTime currentCheckDate = startDate.Value;

            // Start from next day to avoid creating duplicate of parent
            currentCheckDate = currentCheckDate.AddDays(1);

            while (ShouldContinue(parent, currentCheckDate, count + 1))
            {
                if (selectedDays.Contains(currentCheckDate.DayOfWeek))
                {
                    var child = CreateChildTask(parent, currentCheckDate);
                    occurrences.Add(child);
                    count++;
                }

                // If we finished a week and it's Sunday, we skip weeks if interval > 1
                if (currentCheckDate.DayOfWeek == DayOfWeek.Sunday && interval > 1)
                {
                    currentCheckDate = currentCheckDate.AddDays(7 * (interval - 1));
                }

                currentCheckDate = currentCheckDate.AddDays(1);
                
                // Safety break
                if (count >= MAX_OCCURRENCES) break;
            }

            return occurrences;
        }

        private List<TaskModel> GenerateMonthlyOccurrences(TaskModel parent)
        {
            var occurrences = new List<TaskModel>();
            int interval = parent.RecurrenceInterval > 0 ? parent.RecurrenceInterval : 1;
            DateTime? startDate = parent.AssignedDate;

            if (!startDate.HasValue) return occurrences;

            // Determine if we are using Day of Month or Nth Day of Week
            bool isDayOfMonth = true;
            int dayOfMonth = startDate.Value.Day;
            int nth = 0;
            DayOfWeek? dayOfWeek = null;

            if (!string.IsNullOrEmpty(parent.RecurrenceRule))
            {
                if (int.TryParse(parent.RecurrenceRule, out int ruleDay))
                {
                    dayOfMonth = ruleDay;
                }
                else if (parent.RecurrenceRule.Length >= 3)
                {
                    // Handle format like "1MO", "2TU", "-1FR"
                    string dayPart = parent.RecurrenceRule.Substring(parent.RecurrenceRule.Length - 2);
                    string numPart = parent.RecurrenceRule.Substring(0, parent.RecurrenceRule.Length - 2);

                    if (int.TryParse(numPart, out nth))
                    {
                        dayOfWeek = dayPart.ToUpper() switch
                        {
                            "MO" => DayOfWeek.Monday,
                            "TU" => DayOfWeek.Tuesday,
                            "WE" => DayOfWeek.Wednesday,
                            "TH" => DayOfWeek.Thursday,
                            "FR" => DayOfWeek.Friday,
                            "SA" => DayOfWeek.Saturday,
                            "SU" => DayOfWeek.Sunday,
                            _ => (DayOfWeek?)null
                        };

                        if (dayOfWeek.HasValue)
                        {
                            isDayOfMonth = false;
                        }
                    }
                }
            }

            int count = 0;
            DateTime nextMonthDate = startDate.Value;

            while (true)
            {
                nextMonthDate = nextMonthDate.AddMonths(interval);
                DateTime? safeDate = null;

                if (isDayOfMonth)
                {
                    safeDate = GetSafeMonthlyDate(nextMonthDate.Year, nextMonthDate.Month, dayOfMonth);
                }
                else
                {
                    safeDate = GetNthDayOfWeek(nextMonthDate.Year, nextMonthDate.Month, nth, dayOfWeek.Value);
                }
                
                if (!safeDate.HasValue) continue;

                if (!ShouldContinue(parent, safeDate, count + 1)) break;

                var child = CreateChildTask(parent, safeDate.Value);
                occurrences.Add(child);
                count++;

                if (count >= MAX_OCCURRENCES) break;
            }

            return occurrences;
        }

        private DateTime? GetNthDayOfWeek(int year, int month, int nth, DayOfWeek dayOfWeek)
        {
            if (nth == 0) return null;

            if (nth > 0)
            {
                DateTime firstDayOfMonth = new DateTime(year, month, 1);
                int daysOffset = ((int)dayOfWeek - (int)firstDayOfMonth.DayOfWeek + 7) % 7;
                DateTime firstOccurence = firstDayOfMonth.AddDays(daysOffset);
                DateTime nthOccurence = firstOccurence.AddDays((nth - 1) * 7);
                
                if (nthOccurence.Month != month) return null;
                return nthOccurence;
            }
            else // nth < 0, e.g. -1 for last
            {
                int daysInMonth = DateTime.DaysInMonth(year, month);
                DateTime lastDayOfMonth = new DateTime(year, month, daysInMonth);
                int daysOffset = ((int)lastDayOfMonth.DayOfWeek - (int)dayOfWeek + 7) % 7;
                DateTime lastOccurence = lastDayOfMonth.AddDays(-daysOffset);
                
                // For -1 (last), -2 (second to last), etc.
                DateTime nthOccurence = lastOccurence.AddDays((nth + 1) * 7);
                
                if (nthOccurence.Month != month) return null;
                return nthOccurence;
            }
        }

        private List<TaskModel> GenerateYearlyOccurrences(TaskModel parent)
        {
            var occurrences = new List<TaskModel>();
            DateTime? startDate = parent.AssignedDate;
            if (!startDate.HasValue) return occurrences;

            int count = 0;
            DateTime nextDate = startDate.Value;

            while (true)
            {
                nextDate = nextDate.AddYears(1);
                
                // Handle leap year case: if parent was Feb 29, handle safe rollover
                DateTime? safeDate = GetSafeMonthlyDate(nextDate.Year, nextDate.Month, nextDate.Day);
                if (!safeDate.HasValue) break;

                if (!ShouldContinue(parent, safeDate, count + 1)) break;

                var child = CreateChildTask(parent, safeDate.Value);
                occurrences.Add(child);
                count++;

                if (count >= MAX_OCCURRENCES) break;
            }

            return occurrences;
        }

        private bool ShouldContinue(TaskModel parent, DateTime? nextDate, int currentOccurrenceIndex)
        {
            if (currentOccurrenceIndex > MAX_OCCURRENCES) return false;

            // Check Occurrence Count Limit
            if (parent.RecurrenceOccurrenceCount.HasValue && currentOccurrenceIndex >= parent.RecurrenceOccurrenceCount.Value)
            {
                return false;
            }

            // Check End Date Limit
            if (parent.RecurrenceEndDate.HasValue && nextDate.HasValue)
            {
                // We compare dates only, assuming if it's the same day it's still valid
                if (nextDate.Value.Date > parent.RecurrenceEndDate.Value.Date)
                {
                    return false;
                }
            }

            return true;
        }

        private TaskModel CreateChildTask(TaskModel parent, DateTime assignedDate)
        {
            // Calculate time span of original task if exists to maintain duration
            TimeSpan duration = TimeSpan.Zero;
            if (parent.AssignedDate.HasValue && parent.ExpectedDate.HasValue)
            {
                duration = parent.ExpectedDate.Value - parent.AssignedDate.Value;
            }

            var child = new TaskModel
            {
                Title = parent.Title,
                Description = parent.Description,
                InitiatedUserId = parent.InitiatedUserId,
                AssignedUserId = parent.AssignedUserId,
                AssignedDate = assignedDate,
                ExpectedDate = assignedDate.Add(duration),
                TaskStatus = "Pending", // Generated tasks are always pending
                Priority = parent.Priority,
                TaskCategoryId = parent.TaskCategoryId,
                CompanyId = parent.CompanyId,
                
                // Recurrence Metadata
                IsRecurring = true,
                RecurringGroupId = parent.RecurringGroupId,
                RecurrenceParentId = parent.TaskId, // Important: Child points to Parent
                RecurrenceType = parent.RecurrenceType,
                RecurrenceInterval = parent.RecurrenceInterval,
                RecurrenceDays = parent.RecurrenceDays,
                RecurrenceRule = parent.RecurrenceRule,
                RecurrenceEndDate = parent.RecurrenceEndDate,
                RecurrenceOccurrenceCount = parent.RecurrenceOccurrenceCount,
                ReminderOffsetMinutes = parent.ReminderOffsetMinutes,
                Reminder = parent.ReminderOffsetMinutes != null
            };

            // Handle Reminders dynamically based on assigned date and offset
            if (child.Reminder && child.AssignedDate.HasValue && child.ReminderOffsetMinutes.HasValue)
            {
                child.ReminderDate = child.AssignedDate.Value.AddMinutes(child.ReminderOffsetMinutes.Value);
            }

            return child;
        }

        private List<DayOfWeek> ParseWeekdays(string? recurrenceDays)
        {
            if (string.IsNullOrEmpty(recurrenceDays)) return new List<DayOfWeek>();

            var days = new List<DayOfWeek>();
            var parts = recurrenceDays.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var day = part.Trim().ToUpper() switch
                {
                    "MO" => DayOfWeek.Monday,
                    "TU" => DayOfWeek.Tuesday,
                    "WE" => DayOfWeek.Wednesday,
                    "TH" => DayOfWeek.Thursday,
                    "FR" => DayOfWeek.Friday,
                    "SA" => DayOfWeek.Saturday,
                    "SU" => DayOfWeek.Sunday,
                    _ => (DayOfWeek?)null
                };

                if (day.HasValue && !days.Contains(day.Value))
                {
                    days.Add(day.Value);
                }
            }

            return days;
        }

        private DateTime? GetSafeMonthlyDate(int year, int month, int day)
        {
            try
            {
                int maxDays = DateTime.DaysInMonth(year, month);
                if (day > maxDays) return null; // Skip invalid dates as requested
                
                return new DateTime(year, month, day);
            }
            catch
            {
                return null;
            }
        }
    }
}
