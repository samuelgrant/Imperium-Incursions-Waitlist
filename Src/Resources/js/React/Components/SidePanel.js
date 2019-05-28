import React, { Component } from 'react';

// A Side Panel that hides off the side of the screen
// and comes onto the right edge of the screen when active
export class SidePanel extends Component {

    getId() {
        return this.props.id || null;
    }

    getTitle() {
        return this.props.title || null;
    }

    /* Actions: Close */
    hideNav() {
        $('.sidebar-special').removeClass('active');
    }

    render() {
        return (
            <div id={this.getId()} className="sidebar-special">
                <h3 className="pb-4">
                    {this.getTitle()}
                    <i className="fas fa-chevron-double-right float-right" onClick={this.hideNav.bind(this)}></i>
                </h3>

                {this.props.children}
            </div>
        )
    }
}

// A section for a side panel with a heading
export class SideSection extends Component {
    getTitle() {
        return this.props.title || "";
    }

    render() {
        return (
            <div className="col-12 pt-2 pb-3">
                <h5 className="font-alpha">{this.getTitle()}</h5>

                {this.props.children}
            </div>
        );
    }
}

// A button that opens a side panel
export class SidePanelButton extends Component {
    openNav(id) {
        $('.sidebar-special').removeClass('active');
        $(`#${id}`).addClass('active');
    }

    getId() {
        return this.props.id || "";
    }

    getTitle() {
        return this.props.title || "";
    }

    render() {
        return (
            <button className="btn btn-dark mx-1" onClick={this.openNav.bind(this, this.getId())}>{this.getTitle()}</button>
        );
    }
}