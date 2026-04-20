using LWMS.Domain.Enums;

namespace LWMS.Domain.Services;
public class ParcelStateMachine
{
  private static readonly Dictionary<ParcelStatus,List<ParcelStatus>> _transitions = new ()
  {
    {ParcelStatus.Created,new(){ParcelStatus.Picking}},
    {ParcelStatus.Picking,new(){ParcelStatus.Picked}},
    {ParcelStatus.Picked,new(){ParcelStatus.Intrasit}},
    {ParcelStatus.Intrasit,new(){ParcelStatus.ArrivedHub}},
    {ParcelStatus.ArrivedHub,new(){ParcelStatus.OutForDelivery}},
    {ParcelStatus.OutForDelivery,new(){
        ParcelStatus.Delivered,
        ParcelStatus.FailedDelivery
        }},
    {ParcelStatus.FailedDelivery,new(){ParcelStatus.Returning}},
    {ParcelStatus.Returning,new(){ParcelStatus.Returned}},
  };
  public static bool CanTransition(ParcelStatus from,ParcelStatus to)
  {
    if(!_transitions.ContainsKey(from)) return false;
    return _transitions[from].Contains(to);
  }
}