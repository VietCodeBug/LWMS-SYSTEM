using LWMS.Domain.Enums;

namespace LWMS.Domain.Services;
public class ParcelStateMachine
{
  private static readonly Dictionary<ParcelStatus,List<ParcelStatus>> _transitions = new ()
  {
    {ParcelStatus.Created, new() { ParcelStatus.LabelPrinted, ParcelStatus.Picking, ParcelStatus.ArrivedHub, ParcelStatus.Cancelled }},
    {ParcelStatus.LabelPrinted, new() { ParcelStatus.Picking, ParcelStatus.ArrivedHub, ParcelStatus.Cancelled }},
    {ParcelStatus.Picking, new() { ParcelStatus.Picked, ParcelStatus.Cancelled }},
    {ParcelStatus.Picked, new() { ParcelStatus.ArrivedHub, ParcelStatus.Cancelled }},
    {ParcelStatus.ArrivedHub, new() { ParcelStatus.Sorted, ParcelStatus.InBag, ParcelStatus.OutForDelivery, ParcelStatus.Returning }},
    {ParcelStatus.Sorted, new() { ParcelStatus.InBag, ParcelStatus.InTransit }},
    {ParcelStatus.InBag, new() { ParcelStatus.InTransit }},
    {ParcelStatus.InTransit, new() { ParcelStatus.ArrivedHub }},
    {ParcelStatus.OutForDelivery, new() { ParcelStatus.Delivered, ParcelStatus.FailedDelivery }},
    {ParcelStatus.FailedDelivery, new() { ParcelStatus.OutForDelivery, ParcelStatus.Returning }},
    {ParcelStatus.Returning, new() { ParcelStatus.Returned }},
    {ParcelStatus.Returned, new() { }},
    {ParcelStatus.Cancelled, new() { }}
  };
  public static bool CanTransition(ParcelStatus from,ParcelStatus to)
  {
    if(!_transitions.ContainsKey(from)) return false;
    return _transitions[from].Contains(to);
  }
}