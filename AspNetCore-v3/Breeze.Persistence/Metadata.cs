using Breeze.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Breeze.Persistence {
  public class BreezeMetadata {

    public string MetadataVersion { get; set; }
    public string NamingConvention { get; set; }
    public List<MetaType> StructuralTypes {
      get; set;
    }
    /// <summary> Enum types used by entities and complex types </summary>
    public List<MetaEnum> EnumTypes { get; set; }
  }

  /// <summary> Represents a C# enum type, which can be a value for a DataProperty </summary>
  public class MetaEnum {
    /// <summary> Name of the enum </summary>
    public string ShortName { get; set; }
    /// <summary> Namespace of the enum </summary>
    public string Namespace { get; set; }
    /// <summary> String values of the enum </summary>
    public string[] Values { get; set; }
    /// <summary> Int values of the enum </summary>
    public int[] Ordinals { get; set; }
  }

  public class MetaType {

    public MetaType() {
      DataProperties = new List<MetaDataProperty>();
      NavigationProperties = new List<MetaNavProperty>();
      
    }

    
    
    public string ShortName { get; set; }
    public string Namespace { get; set; }
    /// <summary> In the form TypeName:#Namespace </summary>
    public string BaseTypeName { get; set; }

    public AutoGeneratedKeyType? AutoGeneratedKeyType {
      get; set;
    }
    
    public string DefaultResourceName {
      get; set;
    }

    public bool IsComplexType { get; set; }

    
    public List<MetaDataProperty> DataProperties {
      get;set;
    }
    public List<MetaNavProperty> NavigationProperties {
      get; set;
    }
  }

  public class MetaProperty {

    private List<MetaValidator> _validators = new List<MetaValidator>();

    public object Custom { get; set; }

    public string NameOnServer { get; set; }

    public List<MetaValidator> Validators {
      get { return _validators; }
    }

  }

  public class MetaDataProperty : MetaProperty {

    public string DataType { get; set; }
    public string EnumType { get; set; }
    public bool? IsPartOfKey { get; set; }

    public bool? IsNullable { get; set; }

    public int? MaxLength { get; set; }

    public Object DefaultValue { get; set; }

    public string ConcurrencyMode { get; set; }

    public string ComplexTypeName { get; set; }

    // Used with 'Undefined' DataType
    public string RawTypeName { get; set; }

    [NonSerialized]
    public bool IsIdentityColumn;
   
    public void AddValidators(Type clrType) {
      if (!(this.IsNullable ?? false)) {
        Validators.Add(MetaValidator.Required);
      }
      if (this.MaxLength != null) {
        Validators.Add(new MaxLengthMetaValidator(this.MaxLength.Value));
      }
      var validator = MetaValidator.FindValidator(TypeFns.GetNonNullableType(clrType));
      if (validator != null) {
        Validators.Add(validator);
      }
      
    }
  }

  public class MetaNavProperty : MetaProperty {
    public string EntityTypeName { get; set; }
    public bool IsScalar { get; set; }
    public string AssociationName { get; set; }

    public List<String> ForeignKeyNamesOnServer {
      get; set;
    }
    public List<String> InvForeignKeyNamesOnServer {
      get; set;
    }
  }

  public class MetaValidator {

    public MetaValidator(String name) {
      Name = name;
    }
    public static MetaValidator Required = new MetaValidator("required");
    private static Dictionary<Type, MetaValidator> __validatorMap = new Dictionary<Type, MetaValidator>() {
      { typeof(DateTime), new MetaValidator("date") },
      { typeof(DateTimeOffset), new MetaValidator("date") },
      { typeof(Byte), new MetaValidator("byte") },
      { typeof(Int16), new MetaValidator("int16") },
      { typeof(Int32), new MetaValidator("int32") },
      { typeof(Int64), new MetaValidator("int64") },
      { typeof(Single), new MetaValidator("number") },
      { typeof(Double), new MetaValidator("number") },
      { typeof(Decimal), new MetaValidator("number") },
      { typeof(Boolean), new MetaValidator("bool") },
      { typeof(Guid), new MetaValidator("guid") },
      { typeof(TimeSpan), new MetaValidator("duration") },

    };

    public static MetaValidator FindValidator(Type type) {
      MetaValidator validator = null;
      __validatorMap.TryGetValue(type, out validator);
      return validator;
    }

    public string Name {
      get; set;
    }
  }

  public class MaxLengthMetaValidator : MetaValidator {

    public int MaxLength {
      get; set;
    }
    public MaxLengthMetaValidator(int maxLength) : base("maxLength") {
      MaxLength = maxLength;
    }
  }
}
