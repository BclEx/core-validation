import moment from 'moment';
import { nulFormat } from '../globals';

moment.suppressDeprecationWarnings = true;
const _minDateValue = moment([1753, 1, 1]);
const _maxDateValue = moment([9999, 12, 31]);

const getDateAndTime = (text) => {
  let value;
  if ((value = moment(text)).isValid()) return value;
  if ((value = moment(text, 'YYYY-MM-DDTHH:mm')).isValid()) return value;
  return value;
}

const getTime = (text) => {
  let value;
  if (!text.includes(':')) return moment('bad');
  if ((value = moment(text)).isValid()) return value;
  /* istanbul ignore next */
  if ((value = moment(text, 'YYYY-MM-DDTHH:mm')).isValid()) return value;
  /* istanbul ignore next */
  if ((value = moment(text, 'hh:mm:ss a')).isValid()) return value;
  /* istanbul ignore next */
  return value;
}

// date
const DateFormaterDefault = 'YYYY-MM-DD'; //'M/D/YYYY'
export const dateFormater = (value, param) => {
  if (!value) return nulFormat;
  value = moment(value);
  if (param && param.format) {
    switch (param.format) {
      case 'date': return value.format('DD MMMM YYYY');
      case 'longDate': return value.format('dddd, MMMM D, YYYY');
      case 'longDate2': return value.format(value.year() === moment().year() ? 'dddd, MMMM D' : 'dddd, MMMM D, YYYY');
      case 'shortDate': return value.format('D-MMM-YYYY');
      case 'shorterDate': return value.format('MMM D YYYY');
      case 'monthDay': return value.format('MMMM D');
      case 'monthYear': return value.format('MMMM YYYY');
      case 'pattern': return value.format(param.pattern);
      default: throw new Error('param.format invalid');
    }
  }
  return value.format(DateFormaterDefault);
};
export const dateParser = (text, param, error) => {
  if (!text) return [text, true, error];
  let value = moment(text); if (!value.isValid()) return [text, false, error];
  if (value < _minDateValue || value > _maxDateValue) return [value, false, error];
  value = moment([value.year(), value.month(), value.date()]);
  if (param) { // check param
    const minValue = param.minValue; if (minValue && moment(minValue).isAfter(value)) return [value, false, error];
    const maxValue = param.maxValue; if (maxValue && value.isAfter(moment(maxValue))) return [value, false, error];
  }
  return [value, true, error];
};

// dateTime
export const dateTimeFormater = (value, param) => {
  if (!value) return nulFormat;
  value = moment(value);
  if (param && param.format) {
    switch (param.format) {
      case 'dateTime': return value.format('DD MMMM YYYY hh:mm a');
      case 'longDateTime': return value.format('dddd, MMMM D, YYYY hh:mm a');
      case 'longDate': return value.format('dddd, MMMM D, YYYY');
      case 'longTime': return value.format('hh:mm:ss a');
      case 'shortDate': return value.format('D-MMM-YYYY');
      case 'shorterDate': return value.format('MMM D YYYY');
      case 'shortTime': return value.format('hh:mm a');
      case 'tinyDate': return value.format('M/D/YY');
      case 'tinyDateTime': return value.format('M/D/YY hh:mm a');
      case 'pattern': return value.format(param.pattern);
      default: throw new Error('param.format invalid');
    }
  }
  return value.format('MM/DD/YYYY');
};
export const dateTimeParser = (text, param, error) => {
  if (!text) return [text, true, error];
  let value = getDateAndTime(text); if (!value.isValid()) return [text, false, error];
  if (value < _minDateValue || value > _maxDateValue) return [text, false, error];
  if (param) { // check param
    const minValue = param.minValue; if (minValue && moment(minValue).isAfter(value)) return [value, false, error];
    const maxValue = param.maxValue; if (maxValue && value.isAfter(moment(maxValue))) return [value, false, error];
  }
  return [value, true, error];
};

// monthAndDay
export const monthAndDayFormater = (value, param) => {
  if (!value) return nulFormat;
  value = moment(value);
  if (param) {
    switch (param.format) {
      case 'pattern': return value.format(param.pattern);
      default: throw new Error('param.format invalid');
    }
  }
  return value.format('MM/DD');
};
export const monthAndDayParser = (text, param, error) => {
  if (!text) return [text, true, error];
  const match = /^((0[1-9])|(1[0-2]))[\/-](([0-2][0-9])|([3][0-1]))$/.exec(text); if (!match) return [text, false, error];
  let value = moment().set({ year: 2000, month: match[1] - 1, date: match[4], h: 0, m: 0, s: 0 }); //if (!value.isValid()) return [text, false, error];
  return [value, true, error];
};

// time
export const timeFormater = (value, param) => {
  if (!value) return nulFormat;
  value = moment(value);
  if (param && param.format) {
    switch (param.format) {
      case 'longTime': return value.format('hh:mm:ss a');
      case 'shortTime': return value.format('hh:mm a');
      case 'pattern': return value.format(param.pattern);
      default: throw new Error('param.format invalid');
    }
  }
  return value.format('hh:mm ss');
};
export const timeParser = (text, param, error) => {
  if (!text) return [text, true, error];
  let value = getTime(text); if (!value.isValid()) return [text, false, error];
  value = moment({ year: 2000, month: 0, date: 1, h: value.hour(), m: value.minute(), s: value.second() });
  if (param) { // check param
    const minValue = param.minValue; if (minValue) {
      let theValue = moment(minValue, 'hh:mm:ss a');
      theValue = moment({ year: 2000, month: 0, date: 1, h: theValue.hour(), m: theValue.minute(), s: theValue.second() });
      if (theValue.isAfter(value)) return [value, false, error];
    }
    const maxValue = param.maxValue; if (maxValue) {
      let theValue = moment(maxValue, 'hh:mm:ss a');
      theValue = moment({ year: 2000, month: 0, date: 1, h: theValue.hour(), m: theValue.minute(), s: theValue.second() });
      if (value.isAfter(theValue)) return [value, false, error];
    }
  }
  return [value, true, error];
};