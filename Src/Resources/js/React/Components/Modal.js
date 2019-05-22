import React, { Component } from 'react';

export default class Modal extends Component {

    getId() {
        return this.props.id || "";
    }

    getTitle() {
        return this.props.title || "";
    }

    render() {
        let dismiss;
        if (this.props.dismiss) {
            dismiss = (
                <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">×</span>
                </button>
            )
        }

        return (
            <div className="modal fade" id={this.getId()} role="dialog" tabindex="-1">
                <div className="modal-dialog" role="document">
                    <div className="modal-content">

                        <div className="modal-header">
                            <h4 className="modal-title">{this.getTitle()}</h4>
                            {dismiss}
                        </div>

                        <div className="modal-body">
                            {this.props.children}
                        </div>
                    </div>
                </div>
            </div>
        )
    }
}