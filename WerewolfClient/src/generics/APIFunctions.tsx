//Common place to API call function

//import API URL
// const API_URL: string = "https://7c9w77bbx8.execute-api.ap-southeast-2.amazonaws.com/s1/"
const API_URL: string = "https://scoutsonline.com.au/report/"
const token = "lash-impotence-policy-conjure-prototype-activate-deflected"

const messageLevels = [
    "ERROR",
    "EXCEPTION",
    "CONCURRENT",
]

function encodeUrl(url: string) {
    let output: string = ""

    if (!url || url.includes("undefined")) {//Dont sent urls with unknown params
        return output;
    }

    let urlParts: Array<string> = url.split('?')
    if (urlParts[0].includes("http")) {
        output = urlParts[0] + `?token=${token}`
    } else {
        output = API_URL + urlParts[0] + `?token=${token}`
    }

    //If there are parameters
    if (urlParts.length > 1) {
        output += `&${urlParts[1]}`
    }

    return output;
}

function returnToLogin() {
    sessionStorage.removeItem("token")
    localStorage.removeItem("user");
    sessionStorage.setItem("page", "dashboard");
    window.open("/", "_self");
}

function isDuplicate(url: string): boolean {
    const currentCalls = sessionStorage.getItem("currentCalls")
    let newCalls: Array<string> = JSON.parse(currentCalls ?? "[]")

    //If this url doesnt exist
    if (newCalls.length === 0 || !newCalls.includes(url)) {
        //Add the url
        newCalls.push(url)
        sessionStorage.setItem("currentCalls", JSON.stringify(newCalls))
        return false
    } else if (newCalls.includes(url)) {
        return true
    }

    return false
}

function removeCallFromSession(url: string) {
    let newCalls: Array<string> = JSON.parse(sessionStorage.getItem("currentCalls") ?? "[]")
    for (let i in newCalls) {
        if (newCalls[i] === url) {
            newCalls.splice(Number(i))
            sessionStorage.setItem("currentCalls", JSON.stringify(newCalls))
            return
        }
    }
}


async function apiGetCall(url: string, callback: Function, error?: Function) {
    url = encodeUrl(url);

    if (url === "") { return }
    if (isDuplicate(url)) { return }

    fetch(url, {
        method: "GET",
        headers: {
            // authorization: `Bearer ${token}`
            //authorization: `Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IjNieXVSVzFTYmd2NHA0bUdBTVAxUSJ9.eyJpc3MiOiJodHRwczovL2Rldi0xZGtodGFiN2ZidzR4b2lhLnVzLmF1dGgwLmNvbS8iLCJzdWIiOiJTSVQ5SkF0aWhCcWRHNFBNbE9vcEdtTDc1TkNRY0c3a0BjbGllbnRzIiwiYXVkIjoiaHR0cHM6Ly90ZXJyYWZvcm1hcGkvYXBpIiwiaWF0IjoxNjkyNTk1OTgyLCJleHAiOjE2OTI2ODIzODIsImF6cCI6IlNJVDlKQXRpaEJxZEc0UE1sT29wR21MNzVOQ1FjRzdrIiwiZ3R5IjoiY2xpZW50LWNyZWRlbnRpYWxzIn0.jp-0GULD5HLpJrj9AjJUEFr4mxb1fQKaxqJCODcYBhLVdg_GY2yTWcozHBETVpklIuHXFvDmasgcTF0AaH2EpePgNBv4UhVKnIsy6RiBnT0qX5rNG4veIWuxH6Vgp83Ho5BIfyM0Kr8IxMUYlb5xPNe2SFmQHRap5hWZv-jy0ULhQpU0SgnL0AtU4NnfZ8vWSHXq42xzXHWrkMrYYQ9TTzdf3JOoXICABZEg71Rd_5pZsdS65RrlGXX69rJFmzOul_h-0BknX5DMOShF0TECtS50QUKMC51Ug6CFMnYwUGmtGtqQ4SEWzLEB_L32HEqeFaTlxVvwTofGafJpjz_a0A`
        }
    }).then(response => {
        removeCallFromSession(url)
        if (response.ok) {
            return response.json();
        } else if (response.status === 401) {
            return
            returnToLogin();
        } else {
            if (!error) {
                console.error("500")
                return
            }
            return error("500, Backend error")
        }
    }).then(data => {
        if (data) {
            if (data.StatusMessage && data.StatusMessage !== null && messageLevels.includes(data.StatusMessage.MessageLevel)) {
                if (!error) {
                    console.error(data.StatusMessage)
                    return
                }
                return error(data.StatusMessage);
            } else {
                return callback(data);
            }
        } else {
            if (!error) {
                console.error("Error Fetching")
                return
            }
            return error("Error fetching")
        }
    }).catch(e => {
        if (!error) {
            console.error(e)
            return
        }
        return error("Device Offline")
    });
}


function apiBodyCall(url: string, method: string, body: any, callback: Function, error?: Function) {
    url = encodeUrl(url);

    if (url === "") { return; }
    if (isDuplicate(url)) { return }

    fetch(url, {
        method: method,
        body: body,
        headers: {
            "Content-Type": "application/json"
        }
    }).then(response => {
        removeCallFromSession(url)
        if (response.ok) {
            return response.json();
        } else if (response.status === 401) {
            returnToLogin();
        } else {
            response.text().then((data: any) => {
                if (data[0] === "{") {
                    //If this is an error object
                    data = JSON.parse(data);
                    if (!error) {
                        console.error(data.errMsg)
                        return
                    }
                    return error(data.errMsg); //Just pass the error msg
                } else {
                    if (!error) {
                        console.error(data)
                        return
                    }
                    return error(data);
                }
            });
        }
    }).then(data => {
        if (data) {
            //Is there and error the front end should report?
            if (data.MessageLevel && messageLevels.includes(data.MessageLevel)) {
                if (!error) {
                    console.error(data)
                    return
                }
                return error(data);
            }
            else if (data.StatusMessage && data.StatusMessage !== null && messageLevels.includes(data.StatusMessage.MessageLevel)) {
                if (!error) {
                    console.error(data.StatusMessage)
                    return
                }
                return error(data.StatusMessage);
            } else {
                return callback(data);
            }
        } else {
            if (!error) {
                console.error("Error submitting")
                return
            }
            return error("Error submitting")
        }
    }).catch(e => {
        if (e.message.includes("Failed to fetch")) {
            if (!error) {
                console.error("Device Offline")
                return
            }
            return error("Device Offline")
        }
    });
}

//API call with any method but without a body
function apiOtherCall(url: string, method: string, callback: Function, error?: Function) {
    url = encodeUrl(url);

    if (url === "") { return; }
    if (isDuplicate(url)) { return }

    fetch(url, {
        method: method,
    }).then(response => {
        removeCallFromSession(url)
        if (response.ok) {
            return response.json();
        } else if (response.status === 401) {
            returnToLogin();
        } else {
            if (!error) {
                console.error("500")
                return
            }
            return error("500, Backend error")
        }
    }).then(data => {
        if (data) {
            //Is there and error the front end should report?
            if (data.MessageLevel && messageLevels.includes(data.MessageLevel)) {
                if (!error) {
                    console.error(data)
                    return
                }
                return error(data);
            }
            else if (data.StatusMessage && data.StatusMessage !== null && messageLevels.includes(data.StatusMessage.MessageLevel)) {
                if (!error) {
                    console.error(data.StatusMessage)
                    return
                }
                return error(data.StatusMessage);
            } else {
                return callback(data);
            }
        } else {
            if (!error) {
                console.error("Error submitting")
                return
            }
            return error("Error submitting")
        }
    });
}


export {
    apiGetCall,
    apiBodyCall,
    apiOtherCall,
}