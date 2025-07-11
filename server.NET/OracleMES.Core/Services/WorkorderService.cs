using OracleMES.Core.Entities;
using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Exceptions;

namespace OracleMES.Core.Services;

public class WorkorderService
{
    private readonly IWorkorderRepository _workorderRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IProductRepository _productRepository;
    private readonly IWorkcenterRepository _workcenterRepository;

    public WorkorderService(
        IWorkorderRepository workorderRepository,
        IMachineRepository machineRepository,
        IEmployeeRepository employeeRepository,
        IProductRepository productRepository,
        IWorkcenterRepository workcenterRepository)
    {
        _workorderRepository = workorderRepository;
        _machineRepository = machineRepository;
        _employeeRepository = employeeRepository;
        _productRepository = productRepository;
        _workcenterRepository = workcenterRepository;
    }

    // 작업지시 생성
    public async Task<Workorder> CreateWorkorderAsync(Workorder workorder)
    {
        // 유효성 검증
        await ValidateWorkorderAsync(workorder);
        
        // 기본값 설정
        workorder.Status = "Planned";
        workorder.Actualstarttime = null;
        workorder.Actualendtime = null;
        workorder.Actualproduction = 0;
        workorder.Scrap = 0;
        
        return await _workorderRepository.AddAsync(workorder);
    }

    // 작업지시 상태 변경
    public async Task UpdateWorkorderStatusAsync(decimal orderId, string newStatus)
    {
        var workorder = await _workorderRepository.GetByIdAsync(orderId);
        if (workorder == null)
            throw new NotFoundException($"Workorder with ID {orderId} not found");

        // 상태 변경 유효성 검증
        ValidateStatusTransition(workorder.Status, newStatus);

        // 상태별 추가 로직
        switch (newStatus)
        {
            case "In Progress":
                workorder.Actualstarttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                break;
            case "Completed":
                workorder.Actualendtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                break;
            case "Cancelled":
                // 취소 시 재고 반환 로직 등
                break;
        }

        await _workorderRepository.UpdateStatusAsync(orderId, newStatus);
    }

    // 작업지시 시작
    public async Task StartWorkorderAsync(decimal orderId)
    {
        var workorder = await _workorderRepository.GetByIdAsync(orderId);
        if (workorder == null)
            throw new NotFoundException($"Workorder with ID {orderId} not found");

        if (workorder.Status != "Planned" && workorder.Status != "Scheduled")
            throw new InvalidOperationException($"Cannot start workorder in status: {workorder.Status}");

        // 설비 가용성 확인
        var machine = await _machineRepository.GetByIdAsync(workorder.Machineid);
        if (machine?.Status != "Available")
            throw new InvalidOperationException("Machine is not available");

        await UpdateWorkorderStatusAsync(orderId, "In Progress");
    }

    // 작업지시 완료
    public async Task CompleteWorkorderAsync(decimal orderId, decimal actualProduction, decimal scrap)
    {
        var workorder = await _workorderRepository.GetByIdAsync(orderId);
        if (workorder == null)
            throw new NotFoundException($"Workorder with ID {orderId} not found");

        if (workorder.Status != "In Progress")
            throw new InvalidOperationException($"Cannot complete workorder in status: {workorder.Status}");

        // 생산량 검증
        if (actualProduction < 0 || scrap < 0)
            throw new ValidationException("Production and scrap quantities must be non-negative");

        await _workorderRepository.UpdateProductAsync(orderId, actualProduction, scrap);
        await UpdateWorkorderStatusAsync(orderId, "Completed");
    }

    // 우선순위 변경
    public async Task UpdatePriorityAsync(decimal orderId, decimal newPriority)
    {
        if (newPriority < 1 || newPriority > 10)
            throw new ValidationException("Priority must be between 1 and 10");

        await _workorderRepository.UpdatePriorityAsync(orderId, newPriority);
    }

    // 활성 작업지시 조회
    public async Task<IEnumerable<Workorder>> GetActiveWorkordersAsync()
    {
        return await _workorderRepository.GetActiveWorkordersAsync();
    }

    // 오늘 작업지시 조회
    public async Task<IEnumerable<Workorder>> GetTodayWorkordersAsync()
    {
        return await _workorderRepository.GetTodayWorkordersAsync();
    }

    // 지연 작업지시 조회
    public async Task<IEnumerable<Workorder>> GetOverdueWorkordersAsync()
    {
        return await _workorderRepository.GetOverdueWorkordersAsync();
    }

    // 설비별 작업지시 조회
    public async Task<IEnumerable<Workorder>> GetWorkordersByMachineAsync(decimal machineId)
    {
        return await _workorderRepository.GetByMachineAsync(machineId);
    }

    // 직원별 작업지시 조회
    public async Task<IEnumerable<Workorder>> GetWorkordersByEmployeeAsync(decimal employeeId)
    {
        return await _workorderRepository.GetByEmployeeAsync(employeeId);
    }

    // 상태별 작업지시 조회
    public async Task<IEnumerable<Workorder>> GetWorkordersByStatusAsync(string status)
    {
        return await _workorderRepository.GetByStatusAsync(status);
    }

    // 날짜 범위별 작업지시 조회
    public async Task<IEnumerable<Workorder>> GetWorkordersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _workorderRepository.GetByDateRangeAsync(startDate, endDate);
    }

    // 로트번호별 작업지시 조회
    public async Task<IEnumerable<Workorder>> GetWorkordersByLotNumberAsync(string lotNumber)
    {
        return await _workorderRepository.GetByLotNumberAsync(lotNumber);
    }

    // 작업지시 유효성 검증
    private async Task ValidateWorkorderAsync(Workorder workorder)
    {
        // 설비 존재 확인
        var machine = await _machineRepository.GetByIdAsync(workorder.Machineid);
        if (machine == null)
            throw new ValidationException($"Machine with ID {workorder.Machineid} not found");

        // 직원 존재 확인
        var employee = await _employeeRepository.GetByIdAsync(workorder.Employeeid);
        if (employee == null)
            throw new ValidationException($"Employee with ID {workorder.Employeeid} not found");

        // 제품 존재 확인
        var product = await _productRepository.GetByIdAsync(workorder.Productid);
        if (product == null)
            throw new ValidationException($"Product with ID {workorder.Productid} not found");

        // 작업장 존재 확인
        var workcenter = await _workcenterRepository.GetByIdAsync(workorder.Workcenterid);
        if (workcenter == null)
            throw new ValidationException($"Workcenter with ID {workorder.Workcenterid} not found");

        // 수량 검증
        if (workorder.Quantity <= 0)
            throw new ValidationException("Quantity must be greater than 0");

        // 우선순위 검증
        if (workorder.Priority < 1 || workorder.Priority > 10)
            throw new ValidationException("Priority must be between 1 and 10");
    }

    // 상태 전환 유효성 검증
    private void ValidateStatusTransition(string currentStatus, string newStatus)
    {
        var validTransitions = new Dictionary<string, string[]>
        {
            ["Planned"] = new[] { "Scheduled", "In Progress", "Cancelled" },
            ["Scheduled"] = new[] { "In Progress", "Cancelled" },
            ["In Progress"] = new[] { "Completed", "On Hold", "Cancelled" },
            ["On Hold"] = new[] { "In Progress", "Cancelled" },
            ["Completed"] = new[] { "Closed" },
            ["Cancelled"] = new[] { "Planned" }
        };

        if (!validTransitions.ContainsKey(currentStatus) || 
            !validTransitions[currentStatus].Contains(newStatus))
        {
            throw new InvalidOperationException($"Invalid status transition from {currentStatus} to {newStatus}");
        }
    }
} 