import moment from "moment";


function reverseDate(d: string, mdy: boolean): string {//     YYYY/MM/DD <--> DD/MM/YYYY  or   DD/MM/YYYY <-->  MM/DD/YYYY
    if (mdy) {
        d = moment(d).format("MM/DD/YYYY")
    } else {//YMD
        d = moment(d).format("YYYY/MM/DD")
    }
    return d;
}

function formatISOFromDate(d: string): string {//Date() to YYYY-MM-DDTHH:MM:SSZ
    return moment(d).format("YYYY-MM-DDTHH:mm:ss")
}

function formatStandardDate(d: string) {//Date() to DD/MM/YYYY HH:mm:ss
    return moment(d).format("DD/MM/YYYY HH:mm:ss")
}



//Convert a list of data into a list of options for a select field
function formatOptions(data: Array<any>, labelField: string, valueField: string) {
    for (let i in data) {
        data[i] = {
            label: data[i][labelField],
            value: data[i][valueField]
        };
    }

    return data;
}

/*Returns a sub value of an object    e.g.   Engineers[0].EngineerNo returns 5
    oldRow - the object the data is fetched from
    col - use if using col.name
    fieldname - use if not using col.name
*/
function getSubValue(oldRow: any, fieldname: string) {
    const row = JSON.parse(JSON.stringify(oldRow));//Removes any references to the state
    let first: boolean = true;
    var val: string = "";

    if (!fieldname) { return "" }

    if (fieldname.includes('[') || fieldname.includes('.')) {//If this is a sub property/array
        const subNames = fieldname.split('.');
        for (let i in subNames) {
            let subName: string = subNames[i];
            if (subName.includes('[')) {//If grabbing an array element
                const start = subName.indexOf('[');
                const end = subName.indexOf(']');
                const index = Number(subName.substring(start + 1, end - 1 - start));//Index of the array is between the []
                subName = subName.substring(0, start);//Set the subname to just the array name without the index
                if (first) {//Get value from the row
                    val = row[subName][index];
                    first = false;
                } else {//Get value from the altered row
                    if (Array.isArray(val)) {
                        for (let j in val) {
                            val[j] = val[j][subName][index];
                        }
                    } else {
                        val = val[Number(subName)][index];
                    }
                }
            } else {
                if (first) {//Get value from the row
                    val = row[subName];
                    first = false;
                } else {//Get value from the altered row
                    if (Array.isArray(val)) {
                        for (let j in val) {
                            if (val[j]) {
                                val[j] = val[j][subName];
                            }
                        }
                    } else {
                        val = val[Number(subName)];
                    }
                }
            }
            if (!val) {
                break;
            }
        }
    }
    else {
        val = row[fieldname];
    }

    return val;
}

//Sets the subfield value of an object
function setSubValue(obj: any, fieldname: string, newVal: string | number) {
    if (!fieldname.includes(".")) {//No subvalues left to find
        obj[fieldname] = newVal;//Set the value of the final sub field

        return obj;
    } else {//Search for the subfield through a child field
        const childFieldname = fieldname.substr(0, fieldname.indexOf("."));//Name of the next requested child field
        const restFieldname = fieldname.substr(fieldname.indexOf(".") + 1, fieldname.length - fieldname.indexOf(".") - 1);//Rest of the fieldname not yet used

        obj[childFieldname] = setSubValue(obj[childFieldname], restFieldname, newVal);//Recursively search in the child field for the requested field

        return obj;//Final value has been set. Return the parent object
    }
}




interface LatLng {
    long: number,
    lat: number
}

function getLongLat(returnCallback: (latLng: LatLng | null) => void, error: PositionErrorCallback) {
    const callback = (pos: any) => {
        returnCallback({
            long: pos.coords.longitude,
            lat: pos.coords.latitude
        })
    }

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(callback, error)
    } else {
        returnCallback(null)
    }
}

function capsFirstLetter(str: string): string {
    if (!str || str.length === 0) {
        return ""
    }

    return str[0].toUpperCase() + str.substring(1, str.length)
}


export {
    formatISOFromDate,
    formatStandardDate,
    formatOptions,
    reverseDate,
    getSubValue,
    setSubValue,
    getLongLat,
    capsFirstLetter,
};
