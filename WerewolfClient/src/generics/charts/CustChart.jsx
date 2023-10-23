/*
    Custom Chart
    -------------
    Harrison F    May 2021


    

    PROPS
    --------------
    type - { pie, doughnut}
    labels
    data
    dataBackgrounds
    options
      
 */

import React, { Component } from 'react';
import { Pie, Doughnut } from 'react-chartjs-2';


export default class CustChart extends Component {

    render() {
        switch (this.props.type) {
            case "pie":
                return (
                    <Pie
                        data={{
                            labels: this.props.labels,
                            datasets: [
                                {
                                    data: this.props.data,
                                    backgroundColor: this.props.dataBackgrounds
                                }
                            ]
                        }}
                        options={this.props.options}
                    />
                );
            case "doughnut":
                return (
                    <Doughnut
                        data={{
                            labels: this.props.labels,
                            datasets: [
                                {
                                    data: this.props.data,
                                    backgroundColor: this.props.dataBackgrounds
                                }
                            ]
                        }}
                        options={this.props.options}
                    />
                );
            default: 
                return (
                    <Pie
                        data={{
                            labels: this.props.labels,
                            datasets: [
                                {
                                    data: this.props.data,
                                    backgroundColor: this.props.dataBackgrounds
                                }
                            ]
                        }}
                        options={this.props.options}
                    />
                );
        }
    }
}
