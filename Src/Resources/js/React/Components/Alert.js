import React, { Component } from 'react';

export default class Alert extends Component {

    getClass() {
        switch (this.props.type) {
            case "danger":
                return "alert-danger";
            case "success":
                return "alert-success";
            default:
                return "alert-primary";
        }
    }

    render() {
        let dismiss;
        if (this.props.dismiss) {
            dismiss = (
                <button type="button" className="close" data-dismiss="alert" aria-label="Close">
                    <span aria-hidden="true">×</span>
                </button>
            )
        }

        return (
            <div className="col-12">
                <div role="alert" className={`alert ${this.getClass()}`}>
                    {dismiss}

                    <span>{this.props.children}</span>
                </div>
            </div>
        )        
    }
}