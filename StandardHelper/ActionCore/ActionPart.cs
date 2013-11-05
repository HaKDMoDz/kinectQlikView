
using QlikMove.StandardHelper.Enums;
using QlikMove.StandardHelper.EventArguments;
namespace QlikMove.StandardHelper.ActionCore
{
    public interface ActionPart
    {
        /// <summary>
        /// check if the parts has been validated relying on an event that occured
        /// </summary>
        /// <param name="e">the event that occured</param>
        /// <returns>the result as an actionPartResult</returns>
        ActionPartResult checkPart(QlikMoveEventArgs e);
    }
}
