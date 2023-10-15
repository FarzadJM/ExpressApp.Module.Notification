﻿using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Xpo;
using ExpressApp.Module.Notification.Base;
using System.ComponentModel;

namespace ExpressApp.Module.Notification.BusinessObjects;

[Appearance("", AppearanceItemType.ViewItem, "[IsSeen] = False", FontStyle = System.Drawing.FontStyle.Bold, TargetItems = "*", Context = nameof(AppearanceContext.ListView))]
[ImageName("notifications")]
[DeferredDeletion(false)]
[OptimisticLocking(false)]
[XafDisplayName("Notification")]
[Persistent($"gnrl.Notification")]
public class GNRL_Notification : BaseObject
{
    public GNRL_Notification(Session session) : base(session)
    {
    }

    public override void AfterConstruction()
    {
        base.AfterConstruction();

        IsSeen = false;
        IsDelivered = false;
        Level = AlertLevel.Information;
    }

    [VisibleInListView(true)]
    [Size(SizeAttribute.Unlimited)]
    [Persistent("Message")]
    [DbType("varchar(max)")]
    public string Message
    {
        get { return GetPropertyValue<string>(); }
        private set { SetPropertyValue(nameof(Message), value); }
    }

    [VisibleInListView(true)]
    [ModelDefault("DisplayFormat", "{0:yyyy/M/d}")]
    [XafDisplayName("Date")]
    [Persistent("DateCreated")]
    [DbType("datetime2(0)")]
    public DateTime DateCreated
    {
        get { return GetPropertyValue<DateTime>(); }
        private set { SetPropertyValue(nameof(DateCreated), value); }
    }

    [VisibleInListView(false)]
    [XafDisplayName("From")]
    [NoForeignKey]
    [Persistent("FromApplicationUser")]
    public PermissionPolicyUser FromUser
    {
        get { return GetPropertyValue<PermissionPolicyUser>(); }
        private set { SetPropertyValue(nameof(FromUser), value); }
    }

    [SecurityBrowsable]
    [Browsable(false)]
    [NoForeignKey]
    [Persistent("ToApplicationUser")]
    public PermissionPolicyUser ToUser
    {
        get { return GetPropertyValue<PermissionPolicyUser>(); }
        private set { SetPropertyValue(nameof(ToUser), value); }
    }

    [SecurityBrowsable]
    [Browsable(false)]
    [Persistent("IsSeen")]
    [DbType("bit")]
    public bool IsSeen
    {
        get { return GetPropertyValue<bool>(); }
        private set { SetPropertyValue(nameof(IsSeen), value); }
    }

    [SecurityBrowsable]
    [Browsable(false)]
    [Persistent("IsDelivered")]
    [DbType("bit")]
    public bool IsDelivered
    {
        get { return GetPropertyValue<bool>(); }
        private set { SetPropertyValue(nameof(IsDelivered), value); }
    }

    [SecurityBrowsable]
    [Browsable(false)]
    [Persistent("Level")]
    public AlertLevel Level
    {
        get { return GetPropertyValue<AlertLevel>(); }
        private set { SetPropertyValue(nameof(Level), value); }
    }

    [SecurityBrowsable]
    [Browsable(false)]
    [Persistent("ObjectHandle")]
    [DbType("varchar(max)")]
    public string ObjectHandle
    {
        get { return GetPropertyValue<string>(); }
        private set { SetPropertyValue(nameof(ObjectHandle), value); }
    }
}
