using CoreValidation.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using static CoreValidation.Globals;

namespace CoreValidation
{
  public class Validator
  {
    internal Validator(object @this, AbstractBinding binding = null)
    {
      This = @this;
      Binding = binding ?? throw new ArgumentNullException(nameof(binding));
    }

    // STATE
    public readonly object This;
    public readonly AbstractBinding Binding;
    public ICollection<VRule> Rules { get; set; }
    //  public object inputParser = eventTargetInputParser,
    //  public object inputHandler: null,

    public void Reset()
    {
      Binding.SetErrors(This, new { _flag = 0 }.ToParam());
    }
    public bool HasErrors(IDictionary<string, object> opts = null)
    {
      var skipRules = opts != null && opts.TryGetValue("skipRules", out var z) && z is bool v1 ? v1 : false;
      var errors = !skipRules ? RunRules(opts, null, 1) : Binding.GetErrors(This);
      var primaryErrorFlag = ((errors.TryGetValue("_flag", out z) && z is int v2 ? v2 : 0) & 1) != 0;
      return primaryErrorFlag && (errors.Count != 1);
    }
    public bool HasErrorFlag(int bit = 0)
    {
      var errors = Binding.GetErrors(This);
      return ((errors.TryGetValue("_flag", out var z) && z is int v2 ? v2 : 0) & (1 << bit)) != 0;
    }

    public void SetErrorFlag(int bit = 0)
    {
      var errors = Binding.GetErrors(This);
      errors["_flag"] = (errors.TryGetValue("_flag", out var z) && z is int v2 ? v2 : 0) | (1 << bit);
      Binding.SetErrors(This, errors);
    }
    public void ClearErrorFlag(int bit = 0)
    {
      var errors = Binding.GetErrors(This);
      errors["_flag"] = (errors.TryGetValue("_flag", out var z) && z is int v2 ? v2 : 0) & ~(1 << bit);
      Binding.SetErrors(This, errors);
    }

    // RUN
    public ICollection<VRule> GetRules(IDictionary<string, object> opts = null, string field = null)
    {
      var rules = (opts != null && opts.TryGetValue("rules", out var z) && z is ICollection<VRule> v1 ? v1 : null) ?? Rules;
      var state = Binding.GetState(This, opts);
      if (field != null) return rules != null ? new[] { V.Find(state, rules, field) } : null;
      return rules != null ? V.Flatten(state, rules) : new VRule[0];
    }

    public IDictionary<string, object> GetFormats(IDictionary<string, object> opts = null, string field = null)
    {
      var rules = (opts != null && opts.TryGetValue("rules", out var z) && z is ICollection<VRule> v1 ? v1 : null) ?? Rules;
      var state = Binding.GetState(This, opts);
      var values = rules != null ? V.Format(state, rules, field) : new Dictionary<string, object>();
      if (field != null && state.TryGetValue(field, out var s) && values.TryGetValue(field, out var v) && s == v) return new Dictionary<string, object>();
      return values;
    }

    public IDictionary<string, object> RunRules(IDictionary<string, object> opts = null, string field = null, int flag = 0)
    {
      var rules = (opts != null && opts.TryGetValue("rules", out var z) && z is ICollection<VRule> v1 ? v1 : null) ?? Rules;
      var state = Binding.GetState(This, opts);
      var errors = rules != null ? V.Validate(state, rules, field) : new Dictionary<string, object>();
      errors["_flag"] = flag != 0 ? flag : (Binding.GetErrors(This).TryGetValue("_flag", out z) && z is int v2 ? v2 : 0);
      Binding.SetErrors(This, errors);
      return errors;
    }

    public IDictionary<string, object> RunFormats(IDictionary<string, object> opts = null, string field = null)
    {
      var rules = (opts != null && opts.TryGetValue("rules", out var z) && z is ICollection<VRule> v1 ? v1 : null) ?? Rules;
      var state = Binding.GetState(This, opts);
      var values = rules != null ? V.Format(state, rules, field) : new Dictionary<string, object>();
      if (field != null && state.TryGetValue(field, out var s) && values.TryGetValue(field, out var v) && s == v) return new Dictionary<string, object>();
      Binding.SetState(This, opts, values);
      return values;
    }

    public IDictionary<string, object> ReduceState(IDictionary<string, object> opts = null, HashSet<string> exceptFields = null)
    {
      var state = Binding.GetState(This, opts);
      var fieldMap = new HashSet<string>(GetRules(opts).Where(x => exceptFields == null || !exceptFields.Contains(x.Field)).Select(x => x.Field));
      foreach (var key in state.Keys)
        if (!fieldMap.Contains(key) && (exceptFields != null ? !exceptFields.Contains(key) : true))
          state[key] = string.Empty;
      Binding.SetState(This, opts, state);
      return state;
    }

    // BINDERS
    public string LabelFor(string field, IDictionary<string, object> opts = null)
    {
      var rule = GetRules(opts, field).SingleOrDefault() ?? new VRule();
      return rule.Label ?? "Label";
    }

    public object ValueFor(string field, IDictionary<string, object> opts = null)
    {
      var rule = GetRules(opts, field).SingleOrDefault() ?? new VRule();
      var state = Binding.GetState(This, opts);
      var defaultValue = (rule.State.TryGetValue("defaultValue", out var v) ? v : null) ?? NulFormat;
      return (state.TryGetValue(field, out var z) ? z : null) ?? defaultValue;
    }

    public bool RequiredFor(string field, IDictionary<string, object> opts = null)
    {
      var rule = GetRules(opts, field).SingleOrDefault() ?? new VRule();
      var required = rule.Args != null && rule.Args.Any(x => x.Name == "required");
      return required;
    }

    public string ErrorFor(string field, IDictionary<string, object> opts = null)
    {
      var errors = Binding.GetErrors(This);
      var primaryErrorFlag = ((errors.TryGetValue("_flag", out var z) && z is int v2 ? v2 : 0) & 1) != 0;
      return primaryErrorFlag ? errors[field ?? string.Empty] as string ?? string.Empty : null;
    }

    public string FormatFor(string field, IDictionary<string, object> opts = null) =>
      GetFormats(opts, field).TryGetValue(field, out var v) ? v as string : null;

    public object InputFor(string field, IDictionary<string, object> opts = null)
    {
      throw new NotImplementedException();
      //    const inputParser = (opts || {}).inputParser || this.inputParser;
      //    const inputHandler = (opts || {}).inputHandler || this.inputHandler;
      //    if (!inputParser || !inputHandler) throw new Error('inputParser & inputHandler are required');
      //    return (...args) => {
      //      const value = inputParser(args);
      //      inputHandler(this, {
      //        id: field,
      //        value: value,
      //      });
      //    };
    }

    public Action OnBlurFor(string field, IDictionary<string, object> opts = null) =>
      () => { RunFormats(opts, field); };
  }
}
