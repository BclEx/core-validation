import assert from 'power-assert';
import moment from 'moment';

import { nulFormat } from '../src/globals';
import {
    dateFormatter, dateParser,
    dateTimeFormatter, dateTimeParser,
    monthAndDayFormatter, monthAndDayParser,
    timeFormatter, timeParser,
} from '../src/formats/moment';

describe('Date', () => {
    it('should format', () => {
        expect(dateFormatter(null)).toBe(nulFormat);
        expect(dateFormatter('')).toBe(nulFormat);
        expect(dateFormatter('1/1/2017')).toBe('2017-01-01');
        expect(dateFormatter('2017-01-01')).toBe('2017-01-01');
        expect(dateFormatter('2017-01-01 03:00')).toBe('2017-01-01');
    });
    it('should format: *', () => {
        const param = { format: '*' };
        expect(() => dateFormatter('2017-01-01', param)).toThrow();
    });
    it('should format: date', () => {
        const param = { format: 'date' };
        expect(dateFormatter('2017-01-01', param)).toBe('01 January 2017');
    });
    it('should format: longDate', () => {
        const param = { format: 'longDate' };
        expect(dateFormatter('2017-01-01', param)).toBe('Sunday, January 1, 2017');
    });
    it('should format: longDate2', () => {
        const param = { format: 'longDate2' };
        const firstOfYear = moment({ y: moment().year(), M: 1, d: 1 });
        const firstOfYearFormat = firstOfYear.format('dddd, MMMM D');
        expect(dateFormatter(firstOfYear, param)).toBe(firstOfYearFormat);
    });
    it('should format: longDate2', () => {
        const param = { format: 'longDate2' };
        expect(dateFormatter('2013-01-01', param)).toBe('Tuesday, January 1, 2013');
    });
    it('should format: shortDate', () => {
        const param = { format: 'shortDate' };
        expect(dateFormatter('2017-01-01', param)).toBe('1-Jan-2017');
    });
    it('should format: shorterDate', () => {
        const param = { format: 'shorterDate' };
        expect(dateFormatter('2017-01-01', param)).toBe('Jan 1 2017');
    });
    it('should format: monthDay', () => {
        const param = { format: 'monthDay' };
        expect(dateFormatter('2017-01-01', param)).toBe('January 1');
    });
    it('should format: monthYear', () => {
        const param = { format: 'monthYear' };
        expect(dateFormatter('2017-01-01', param)).toBe('January 2017');
    });
    it('should format: pattern', () => {
        const param = { format: 'pattern', pattern: 'YYYY' };
        expect(dateFormatter('2017-01-01', param)).toBe('2017');
    });
    it('should parse', () => {
        expect(dateParser(null)).toEqual([null, true, undefined]);
        expect(dateParser('')).toEqual(['', true, undefined]);
        expect(dateParser('1ab')).toEqual(['1ab', false, undefined]);
        expect(dateParser('blah').toString()).toEqual('blah,false,');
        expect(dateParser('3:00 pm').toString()).toEqual('3:00 pm,false,');
        expect(dateParser('2012-01-01 3pm').toString()).toEqual('2012-01-01 3pm,false,');
        expect(dateParser('1/1/2012').toString()).toEqual('Sun Jan 01 2012 00:00:00 GMT-0600,true,');
        expect(dateParser('2012-01-01').toString()).toEqual('Sun Jan 01 2012 00:00:00 GMT-0600,true,');
        expect(dateParser('2012-01-01 3:00 pm').toString()).toEqual('Sun Jan 01 2012 00:00:00 GMT-0600,true,');
        expect(dateParser('1901-01-01 03:00:00 am').toString()).toEqual('Tue Jan 01 1901 00:00:00 GMT-0600,true,');
        // bounds-check
        expect(dateParser('1752-01-01').toString()).toEqual('1752-01-01,false,');
        expect(dateParser('9999-01-01').toString()).toEqual('9999-01-01,false,');
    });
    it('should parse: minValue', () => {
        expect(dateParser('2011-01-01', { minValue: '2012-01-01' }).toString()).toEqual('2011-01-01,false,');
        expect(dateParser('2012-01-01', { minValue: '1/1/2011' }).toString()).toEqual('Sun Jan 01 2012 00:00:00 GMT-0600,true,');
    });
    it('should parse: maxValue', () => {
        expect(dateParser('2017-01-01', { maxValue: '2012-01-01' }).toString()).toEqual('2017-01-01,false,');
        expect(dateParser('2011-01-01', { maxValue: '1/1/2012' }).toString()).toEqual('Sat Jan 01 2011 00:00:00 GMT-0600,true,');
    });
});

describe('DateTime', () => {
    it('should format', () => {
        expect(dateTimeFormatter(null)).toBe(nulFormat);
        expect(dateTimeFormatter('')).toBe(nulFormat);
        expect(dateTimeFormatter('1/1/2017')).toBe('2017-01-01 12:00 am');
        expect(dateTimeFormatter('2017-03-02')).toBe('2017-03-02 12:00 am');
        expect(dateTimeFormatter('2017-01-01 03:00')).toBe('2017-01-01 3:00 am');
    });
    it('should format: *', () => {
        const param = { format: '*' };
        expect(() => dateTimeFormatter('2017-01-01 03:00 am', param)).toThrow();
    });
    it('should format: pattern', () => {
        const param = { format: 'pattern', pattern: 'YYYY' };
        expect(dateTimeFormatter('2017-01-01', param)).toBe('2017');
    });
    it('should format: dateTime', () => {
        const param = { format: 'dateTime' };
        expect(dateTimeFormatter('2017-01-01 03:00 am', param)).toBe('01 January 2017 3:00 am');
    });
    it('should format: longDateTime', () => {
        const param = { format: 'longDateTime' };
        expect(dateTimeFormatter('2017-01-01 03:00 am', param)).toBe('Sunday, January 1, 2017 3:00 am');
    });
    it('should format: longDate', () => {
        const param = { format: 'longDate' };
        expect(dateTimeFormatter('2017-01-01', param)).toBe('Sunday, January 1, 2017');
    });
    it('should format: longTime', () => {
        const param = { format: 'longTime' };
        expect(dateTimeFormatter('2017-01-01 03:00:00 am', param)).toBe('03:00:00 am');
    });
    it('should format: shortDate', () => {
        const param = { format: 'shortDate' };
        expect(dateTimeFormatter('2017-01-01', param)).toBe('1-Jan-2017');
    });
    it('should format: shorterDate', () => {
        const param = { format: 'shorterDate' };
        expect(dateTimeFormatter('2017-01-01', param)).toBe('Jan 1 2017');
    });
    it('should format: shortTime', () => {
        const param = { format: 'shortTime' };
        expect(dateTimeFormatter('2017-01-01 03:00:00 am', param)).toBe('3:00 am');
    });
    it('should format: tinyDate', () => {
        const param = { format: 'tinyDate' };
        expect(dateTimeFormatter('2017-01-01 03:00:00 am', param)).toBe('1/1/17');
    });
    it('should format: tinyDateTime', () => {
        const param = { format: 'tinyDateTime' };
        expect(dateTimeFormatter('2017-01-01 03:00:00 am', param)).toBe('1/1/17 3:00 am');
    });
    it('should parse', () => {
        expect(dateTimeParser(null)).toEqual([null, true, undefined]);
        expect(dateTimeParser('')).toEqual(['', true, undefined]);
        expect(dateTimeParser('1ab').toString()).toEqual('1ab,false,');
        expect(dateTimeParser('blah').toString()).toEqual('blah,false,');
        expect(dateTimeParser('2017-01-01 03:00:00 am').toString()).toEqual('Sun Jan 01 2017 03:00:00 GMT-0600,true,');
        expect(dateTimeParser('1901-01-01 03:00:00 am').toString()).toEqual('Tue Jan 01 1901 03:00:00 GMT-0600,true,');
    });
    it('should parse: minValue', () => {
        expect(dateTimeParser('2011-01-01', { minValue: '2012-01-01' }).toString()).toEqual('2011-01-01,false,');
        expect(dateTimeParser('2012-01-01', { minValue: '1/1/2011' }).toString()).toEqual('Sun Jan 01 2012 00:00:00 GMT-0600,true,');
    });
    it('should parse: maxValue', () => {
        expect(dateTimeParser('2017-01-01', { maxValue: '2012-01-01' }).toString()).toEqual('2017-01-01,false,');
        expect(dateTimeParser('2011-01-01', { maxValue: '1/1/2012' }).toString()).toEqual('Sat Jan 01 2011 00:00:00 GMT-0600,true,');
    });
});

describe('MonthAndDay', () => {
    it('should format', () => {
        expect(monthAndDayFormatter(null)).toBe(nulFormat);
        expect(monthAndDayFormatter('')).toBe(nulFormat);
        expect(monthAndDayFormatter('1/1/2017')).toBe('01/01');
        expect(monthAndDayFormatter('2017-03-02')).toBe('03/02');
        expect(monthAndDayFormatter('2017-01-01 03:00')).toBe('01/01');
    });
    it('should format: *', () => {
        const param = { format: '*' };
        expect(() => monthAndDayFormatter('2017-01-01', param)).toThrow();
    });
    it('should format: pattern', () => {
        const param = { format: 'pattern', pattern: 'YYYY' };
        expect(monthAndDayFormatter('2017-01-01', param)).toBe('2017');
    });
    it('should parse', () => {
        expect(monthAndDayParser(null)).toEqual([null, true, undefined]);
        expect(monthAndDayParser('')).toEqual(['', true, undefined]);
        expect(monthAndDayParser('1ab')).toEqual(['1ab', false, undefined]);
        expect(monthAndDayParser('blah').toString()).toEqual('blah,false,');
        expect(monthAndDayParser('31/12').toString()).toEqual('31/12,false,');
        expect(monthAndDayParser('12/40').toString()).toEqual('12/40,false,');
        expect(monthAndDayParser('12/31').toString()).toEqual('Sun Dec 31 2000 00:00:00 GMT-0600,true,');
        expect(monthAndDayParser('12/31 3:00 pm').toString()).toEqual('12/31 3:00 pm,false,');
        expect(monthAndDayParser('00/00').toString()).toEqual('00/00,false,');
    });
});

describe('Time', () => {
    it('should format', () => {
        expect(timeFormatter(null)).toBe(nulFormat);
        expect(timeFormatter('')).toBe(nulFormat);
        expect(timeFormatter('1/1/2017')).toEqual('12:00 am');
        expect(timeFormatter('2017-01-01')).toEqual('12:00 am');
        expect(timeFormatter('2017-01-01 03:00')).toEqual('3:00 am');
    });
    it('should format: *', () => {
        const param = { format: '*' };
        expect(() => timeFormatter('2017-01-01', param)).toThrow();
    });
    it('should format: longTime', () => {
        const param = { format: 'longTime' };
        expect(timeFormatter('2017-01-01', param)).toEqual('12:00:00 am');
    });
    it('should format: shortTime', () => {
        const param = { format: 'shortTime' };
        expect(timeFormatter('2017-01-01 17:00', param)).toEqual('5:00 pm');
    });
    it('should format: pattern', () => {
        const param = { format: 'pattern', pattern: 'hh a' };
        expect(timeFormatter('2017-01-01 17:00', param)).toEqual('05 pm');
    });
    it('should parse', () => {
        expect(timeParser(null)).toEqual([null, true, undefined]);
        expect(timeParser('')).toEqual(['', true, undefined]);
        expect(timeParser('1ab').toString()).toEqual('1ab,false,');
        expect(timeParser('blah').toString()).toEqual('blah,false,');
        expect(timeParser('3:00 pm').toString()).toEqual('Sat Jan 01 2000 15:00:00 GMT-0600,true,');
        expect(timeParser('2012-01-01 3:11:01 pm').toString()).toEqual('Sat Jan 01 2000 15:11:01 GMT-0600,true,');
        expect(timeParser('abc 3:00').toString()).toEqual('Sat Jan 01 2000 03:00:00 GMT-0600,true,');
    });
    it('should parse: minValue', () => {
        expect(timeParser('2012-01-01 3:11:01 pm', { minValue: '5:00:00 pm' }).toString()).toEqual('2012-01-01 3:11:01 pm,false,');
        expect(timeParser('2012-01-01 3:11:01 pm', { minValue: '1:00:00 am' }).toString()).toEqual('Sat Jan 01 2000 15:11:01 GMT-0600,true,');
    });
    it('should parse: maxValue', () => {
        expect(timeParser('2012-01-01 3:11:01 pm', { maxValue: '1:00:00 am' }).toString()).toEqual('2012-01-01 3:11:01 pm,false,');
        expect(timeParser('2012-01-01 3:11:01 pm', { maxValue: '5:00:00 pm' }).toString()).toEqual('Sat Jan 01 2000 15:11:01 GMT-0600,true,');
    });
});
