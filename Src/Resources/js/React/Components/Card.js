import React, { Component } from 'react';

export default class Card extends Component {


    render() {
        return (
            <div className="card">
                <div className="card-header">
                    <h5 className="mb-0">{this.props.heading}</h5>
                </div>
                <div className="card-body">
                    {this.props.children}
                </div>
            </div>
        )
    }
}