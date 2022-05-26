namespace InferenceService.Contracts;

public class WorkerTask
{
	public Guid TaskId { get; set; }
	public WorkerTaskStatus Status { get; set; }
	public string WorkItem { get; set; }
	public string Result { get; set; }
}
