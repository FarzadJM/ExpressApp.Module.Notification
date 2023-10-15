﻿using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using ExpressApp.Module.Notification.Base;
using ExpressApp.Module.Notification.BusinessObjects;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressApp.Module.Notification.Controllers.Notification
{
    public partial class ListViewController : ObjectViewController<ListView, GNRL_Notification>
    {
        private readonly INotificationDelivery notificationDelivery;

        public ListViewController()
        {
            InitializeComponent();
        }

        [ActivatorUtilitiesConstructor]
        public ListViewController(INotificationDelivery notificationDelivery) : this()
        {
            this.notificationDelivery = notificationDelivery;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            var c1 = CriteriaOperator.Parse($"IsCurrentUserId([ToUser.Oid])");
            var c2 = CriteriaOperator.Parse($"[IsDelivered] = False");
            var c3 = new GroupOperator(c1, c2);
            
            var objectSpace = Application.CreateObjectSpace(typeof(GNRL_Notification));
            var notifications = objectSpace.GetObjects<GNRL_Notification>(c3);
            
            foreach (var notification in notifications)
            {
                notification.SetMemberValue(nameof(GNRL_Notification.IsDelivered), true);
            }

            try
            {
                objectSpace.CommitChanges();
            }
            catch (Exception)
            {
                objectSpace.Rollback();
                objectSpace.Refresh();
            }

            View.AllowNew.SetItemValue(string.Empty, false);
            View.AllowEdit.SetItemValue(string.Empty, false);

            ObjectSpace.ObjectDeleted += ObjectSpace_ObjectDeleted;

            Frame.GetController<ListViewProcessCurrentObjectController>().CustomizeShowViewParameters += NotificationViewController_CustomizeShowViewParameters;
        }

        private void ObjectSpace_ObjectDeleted(object sender, ObjectsManipulatingEventArgs e)
        {
            foreach (GNRL_Notification item in e.Objects)
            {
                if (!item.IsSeen)
                {
                    notificationDelivery.NotifyDismiss(item.Oid, item.ToUser.Oid);
                }
            }
        }

        void NotificationViewController_CustomizeShowViewParameters(object sender, CustomizeShowViewParametersEventArgs e)
        {
            var nestedObjectSpace = ObjectSpace.CreateNestedObjectSpace();

            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Context = TemplateContext.PopupWindow;
            e.ShowViewParameters.Controllers.Add(Application.CreateController<DialogController>());
            e.ShowViewParameters.CreatedView = Application.CreateDetailView(nestedObjectSpace, nestedObjectSpace.GetObject(ViewCurrentObject));

            if (!ViewCurrentObject.IsSeen)
            {
                ViewCurrentObject.SetMemberValue(nameof(GNRL_Notification.IsSeen), true);
                ViewCurrentObject.SetMemberValue(nameof(GNRL_Notification.IsDelivered), true);

                ObjectSpace.CommitChanges();

                if (ViewCurrentObject.ToUser is not null)
                {
                    notificationDelivery.NotifyDismiss(ViewCurrentObject.Oid, ViewCurrentObject.ToUser.Oid);
                }
            }
        }

        protected override void OnDeactivated()
        {
            ObjectSpace.ObjectDeleted -= ObjectSpace_ObjectDeleted;

            Frame.GetController<ListViewProcessCurrentObjectController>().CustomizeShowViewParameters -= NotificationViewController_CustomizeShowViewParameters;

            base.OnDeactivated();
        }
    }
}
