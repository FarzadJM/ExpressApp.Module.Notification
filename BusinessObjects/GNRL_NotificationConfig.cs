﻿using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ExpressApp.Module.Notification.BusinessObjects;

[ModelDefault("IsCloneable", "True")]
[Appearance("", AppearanceItemType.ViewItem, "[TargetType] Is Null", Enabled = false, TargetItems = nameof(Criteria))]
[ImageName("Actions_Settings")]
[DefaultClassOptions]
[NavigationItem(false)]
[DeferredDeletion(false)]
[XafDisplayName("Notification Config")]
[Persistent($"gnrl.NotificationConfig")]
public abstract class GNRL_NotificationConfig : BaseObject
{
    private Type targetType;

    public GNRL_NotificationConfig(Session session) : base(session)
    {
    }

    [RuleRequiredField]
    [Size(SizeAttribute.DefaultStringMappingFieldSize)]
    [Persistent("Name")]
    [DbType("varchar(200)")]
    public string Name
    {
        get { return GetPropertyValue<string>(); }
        set { SetPropertyValue(nameof(Name), value); }
    }

    [RuleRequiredField]
    [ToolTip("This text will be evaluated by criteria language syntax. You can use 'Target Type' members.")]
    [Size(SizeAttribute.Unlimited)]
    [Persistent("Message")]
    [DbType("varchar(max)")]
    public string Message
    {
        get { return GetPropertyValue<string>(); }
        set { SetPropertyValue(nameof(Message), value); }
    }

    [IgnoreDataMember]
    [RuleRequiredField]
    [TypeConverter(typeof(SecurityTargetTypeConverter))]
    [ImmediatePostData]
    [NonPersistent]
    public Type TargetType
    {
        get
        {
            if (targetType == null && !string.IsNullOrWhiteSpace(TargetTypeFullName))
            {
                if (TargetTypeFullName is null)
                {
                    targetType = null;
                }
                else
                {
                    ITypesInfo typesInfo = XafTypesInfo.Instance;
                    targetType = typesInfo.FindTypeInfo(TargetTypeFullName)?.Type;
                }
            }

            return targetType;
        }
        set
        {
            targetType = value;
            TargetTypeFullName = value?.FullName;

            OnChanged(nameof(TargetType));
        }
    }

    //[XafDisplayName("Members")]
    //[Size(SizeAttribute.Unlimited)]
    //public string ObjMembers
    //{
    //    get
    //    {
    //        if (TargetType != null)
    //        {
    //            ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(TargetType);
    //            return typeInfo.Members.Where(x => x.IsVisible || x.IsAttributeDefined<SecurityBrowsableAttribute>(recursive: true)).Select(x => x.Name).Aggregate((x, y) => $"{x}; {y}");
    //        }
    //        return string.Empty;
    //    }
    //}

    [RuleRequiredField(DefaultContexts.Save, TargetCriteria = "[TargetType] Is Not Null")]
    [CriteriaOptions(nameof(TargetType))]
    [EditorAlias("PopupCriteriaPropertyEditor")]
    [Size(SizeAttribute.Unlimited)]
    [Persistent("Criteria")]
    [DbType("varchar(max)")]
    public string Criteria
    {
        get { return GetPropertyValue<string>(); }
        set { SetPropertyValue(nameof(Criteria), value); }
    }

    [RuleRequiredField]
    [XafDisplayName("Recipients")]
    [Association]
    [DevExpress.Xpo.Aggregated]
    public XPCollection<GNRL_NotificationRecipientConfig> Recipients => GetCollection<GNRL_NotificationRecipientConfig>();

    [Browsable(false)]
    [Persistent("TargetType")]
    [DbType("varchar(max)")]
    [ObjectValidatorIgnoreIssue(new Type[] { typeof(ObjectValidatorLargeNonDelayedMember) })]
    public string TargetTypeFullName
    {
        get { return GetPropertyValue<string>(); }
        set { SetPropertyValue(nameof(TargetTypeFullName), value); }
    }

    //[Size(SizeAttribute.Unlimited)]
    //public string MessageSample
    //{
    //    get
    //    {
    //        try
    //        {
    //            return Convert.ToString(Session.Evaluate(TargetType, CriteriaOperator.TryParse(Message), CriteriaOperator.TryParse(string.Empty)));
    //        }
    //        catch (Exception ex)
    //        {
    //            return ex.Message;
    //        }
    //    }
    //}
}
