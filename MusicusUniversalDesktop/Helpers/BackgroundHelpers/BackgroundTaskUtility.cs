﻿using System;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace Musicus.Helpers.BackgroundHelpers
{
    public class BackgroundTaskUtility
    {

        public const string ServicingCompleteTaskEntryPoint = "Musicus.BackgroundPlayer.AudioTask";
        public const string ServicingCompleteTaskName = "AudioTask";
        public static bool ServicingCompleteTaskRegistered = false;

        /// <summary>

        /// Register a background task with the specified taskEntryPoint, name, trigger,

        /// and condition (optional).

        /// </summary>

        /// <param name="taskEntryPoint">Task entry point for the background task.</param>

        /// <param name="name">A name for the background task.</param>

        /// <param name="trigger">The trigger for the background task.</param>

        /// <param name="condition">An optional conditional event that must be true for the task to fire.</param>

        public static BackgroundTaskRegistration RegisterBackgroundTask(String taskEntryPoint, String name, IBackgroundTrigger trigger, IBackgroundCondition condition)
        {
            if (TaskRequiresBackgroundAccess(name))
            {
                // If the user denies access, the task will not run.
                var requestTask = BackgroundExecutionManager.RequestAccessAsync();
            }

            var builder = new BackgroundTaskBuilder();
            builder.Name = name;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            if (condition != null)
            {
                builder.AddCondition(condition);
                //
                // If the condition changes while the background task is executing then it will
                // be canceled.
                //
                builder.CancelOnConditionLoss = true;
            }

            BackgroundTaskRegistration task = builder.Register();
            UpdateBackgroundTaskRegistrationStatus(name, true);

            //
            // Remove previous completion status.
            //

            var settings = ApplicationData.Current.LocalSettings;
            settings.Values.Remove(name);
            return task;

        }



        /// <summary>
        /// Unregister background tasks with specified name.
        /// </summary>
        /// <param name="name">Name of the background task to unregister.</param>

        public static void UnregisterBackgroundTasks(String name)
        {
            //
            // Loop through all background tasks and unregister any with SampleBackgroundTaskName or
            // SampleBackgroundTaskWithConditionName.
            //

            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == name)
                {
                    cur.Value.Unregister(true);
                }
            }

            UpdateBackgroundTaskRegistrationStatus(name, false);
        }



        /// <summary>
        /// Store the registration status of a background task with a given name.
        /// </summary>
        /// <param name="name">Name of background task to store registration status for.</param>
        /// <param name="registered">TRUE if registered, FALSE if unregistered.</param>

        public static void UpdateBackgroundTaskRegistrationStatus(String name, bool registered)
        {
            switch (name)
            {
                case ServicingCompleteTaskName:
                    ServicingCompleteTaskRegistered = registered;
                    break;
            }

        }



        /// <summary>
        /// Get the registration / completion status of the background task with 
        /// given name.
        /// </summary>
        /// <param name="name">Name of background task to retreive registration status.</param>

        public static String GetBackgroundTaskStatus(String name)
        {
            var registered = false;
            switch (name)
            {        
                case ServicingCompleteTaskName:
                    registered = ServicingCompleteTaskRegistered;
                    break;
            }

            var status = registered ? "Registered" : "Unregistered";

            object taskStatus;
            var settings = ApplicationData.Current.LocalSettings;

            if (settings.Values.TryGetValue(name, out taskStatus))
            {
               status += " - " + taskStatus.ToString();
            }

            return status;

        }


        public static bool TaskRequiresBackgroundAccess(String name)
        {
            if (name == ServicingCompleteTaskName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
