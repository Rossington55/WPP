/*
    Name
    -------------
    Harrison F    June 2020


    

    PROPS
    --------------
    *title
    *description
    icon
    style
    pageName - if set to a value, clicking on this will redirect to that page
    chart
      
 */

import React, { Component } from 'react';

const styles = {
    title: {
        fontWeight: "bold"
    },
}


export default class StatCard extends Component {

    constructor(props) {
        super(props);
        const hasCustomColour = this.props.style && this.props.style.backgroundColor;
        const hasPage = this.pageAuthorized();

        this.state = {
            hasPage: hasPage,
            style: {
                container: {
                    minWidth: 200,
                    padding: "8%",
                    color: hasCustomColour ? "white" : "black",
                    cursor: hasPage ? "pointer" : "",
                },
                description: {
                    fontWeight: "bold",
                    marginBottom: 0,
                    color: hasCustomColour ? "white" : "#6a6a6a"
                }
            }
        }
    }

    pageAuthorized = () => {
        const page = this.props.pageName;
        const role = JSON.parse(sessionStorage.getItem("role"));


        for (let i in role.viewList) {
            if (role.viewList[i].ModuleView.ViewName === page) {
                return true;
            }
        }

        return false;
    }

    handleClick = () => {
        if (!this.state.hasPage) {
            return;
        }
        window.open(`/main?page=${this.props.pageName}`, "_self");
    }

    render() {
        const { style } = this.state;
        const createIcon = () => {
            let icon = this.props.icon;

            if (!icon) { return (null); }

            icon = {
                ...icon,
                ...{
                    props: {
                        ...icon.props,
                        ...{
                            fontSize: "large",
                            style: {
                                ...icon.props.style,
                                ...{
                                    marginBottom: "10%"
                                }
                            }
                        }
                    }
                }
            };
            return (icon);

        }
        return (
            <article
                className={`paper ${this.state.hasPage ? "hoverable" : ""}`}
                style={{
                    ...style.container,
                    ...this.props.style,
                }}
                onClick={this.handleClick}
            >

                {createIcon()}

                {/*Title and description*/}
                <h2 style={styles.title}>{this.props.title}</h2>
                <p style={style.description}>{this.props.description}</p>

                {/*Chart*/}
                {this.props.chart}
            </article>
        );
    }
}
